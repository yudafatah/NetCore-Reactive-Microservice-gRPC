using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProductGrpc.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProductWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration configuration;
        private readonly ProductFactory productFactory;

        public Worker(ILogger<Worker> logger, IConfiguration configuration, ProductFactory productFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.productFactory = productFactory ?? throw new ArgumentNullException(nameof(productFactory));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                using var channel = GrpcChannel.ForAddress(configuration["WorkerService:ServerUrl"]);
                var client = new ProductProtoService.ProductProtoServiceClient(channel);

                // to generate product repeatedly

                //_logger.LogInformation("Add Product started");
                //var addProductRes = await client.AddProductAsync(await productFactory.Generate());
                //_logger.LogInformation("Add Product started: {product}",addProductRes.ToString());

                await Task.Delay(configuration.GetValue<int>("WorkerService:TaskInterval"), stoppingToken);
            }
        }
    }
}
