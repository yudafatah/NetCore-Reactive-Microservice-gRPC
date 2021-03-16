using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProductGrpc.Protos;
using System;
using System.Threading.Tasks;

namespace ProductWorkerService
{
    public class ProductFactory
    {
        private readonly ILogger<ProductFactory> logger;
        private readonly IConfiguration configuration;

        public ProductFactory(ILogger<ProductFactory> logger, IConfiguration configuration)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public Task<AddProductReq> Generate()
        {
            return Task.FromResult(new AddProductReq
            {
                Product = new ProductModel
                {
                    Description = "tes",
                    Name = "tes",
                    Price = new Random().Next(10000),
                    Status = ProductStatus.Instock
                }
            });
        }
    }
}
