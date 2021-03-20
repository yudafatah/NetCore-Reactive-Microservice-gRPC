using Grpc.Core;
using Grpc.Net.Client;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ShoppingCartGrpc.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingCartWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration configuration;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // testing only: waiting server to up
            Thread.Sleep(5000);

            while (!stoppingToken.IsCancellationRequested)
            {
                using var scChannel = GrpcChannel.ForAddress(configuration["WorkerService:ShoppingCartGrpcUrl"]);
                var scClient = new ShoppingCartProtoService.ShoppingCartProtoServiceClient(scChannel);

                // get Token from IDS4
                var token = await GetTokenIDS4();

                // cek exist or not, if not then create new one
                var scModel = await GetOrCreateSCAsync(scClient, token);

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(configuration.GetValue<int>("WorkerService:TaskInterval"), stoppingToken);
            }
        }

        private async Task<string> GetTokenIDS4()
        {
            // discover endpoints from metadata
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5005");
            if (disco.IsError)
            {
                _logger.LogInformation("discovery endpoint from IDS4 error");
                return string.Empty;
            }

            // req token
            var tokenRes = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "ShoppingCartClient",
                ClientSecret = "secret",
                Scope = "ShoppingCartAPI"
            });

            if (tokenRes.IsError)
            {
                _logger.LogInformation("generate token from from IDS4 error");
                return string.Empty;
            }

            _logger.LogInformation("success generate token from from IDS4");

            return tokenRes.AccessToken;
        }

        private async Task<ShoppingCartModel> GetOrCreateSCAsync(ShoppingCartProtoService.ShoppingCartProtoServiceClient scClient,
            string token)
        {
            ShoppingCartModel shoppingCartModel;

            // assign header Metadata
            var headers = new Metadata();
            headers.Add("Authorization", $"Bearer {token}");

            try
            {
                _logger.LogInformation("worker Get Shopping Cart starting..");

                shoppingCartModel = await scClient.GetShoppingCartAsync( new GetShoppingCartReq
                {
                    Username = configuration["WorkerService:Username"]
                }, headers);
                _logger.LogInformation("Get Shopping Cart Response : {response}", shoppingCartModel.ToString());
            }
            catch (RpcException exception)
            {
                if(exception.StatusCode == StatusCode.NotFound)
                {
                    _logger.LogInformation("worker Create Shopping Cart starting..");
                    shoppingCartModel = await scClient.CreateShoppingCartAsync(new ShoppingCartModel
                    {
                        Username = configuration["WorkerService:Username"]
                    }, headers);
                    _logger.LogInformation("Create Shopping Cart Response : {response}",shoppingCartModel.ToString());
                }
                else
                {
                    throw;
                }
            }

            return shoppingCartModel;
        }
    }
}
