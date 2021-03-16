using Grpc.Core;
using ProductGrpc.Protos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EcommGrpcClient.Pages
{
    public partial class Index
    {
        private IList<ProductModel> productModels;

        protected override async Task OnInitializedAsync()
        {
            // for testing : waiting grpc server up
            System.Threading.Thread.Sleep(5000);

            var client = new ProductProtoService.ProductProtoServiceClient(Channel);

            // request param
            var getAllProducts = new GetAllProductReq();

            // get all product streaming data from grpc
            var resStream = client.GetAllProduct(getAllProducts);

            // add each stream to productModels
            List<ProductModel> temp = new List<ProductModel>();
            await foreach (var item in resStream.ResponseStream.ReadAllAsync())
            {
                temp.Add(item);

                Console.WriteLine(item);
            }

            productModels = temp;
        }

        // example
        public async Task AddProduct()
        {
            var client = new ProductProtoService.ProductProtoServiceClient(Channel);

            var addProductRes = await client.AddProductAsync(
                new AddProductReq
                {
                    Product = new ProductModel
                    {
                        Name = "Red",
                        Description = "Des",
                        Price = 1000,
                        Status = ProductStatus.Instock
                    }
                }
            );
        }

        // example
        public async Task UpdateProduct()
        {
            var client = new ProductProtoService.ProductProtoServiceClient(Channel);

            var addProductRes = await client.UpdateProductAsync(
                new UpdateProductReq
                {
                    Product = new ProductModel
                    {
                        Name = "Red",
                        Description = "Des",
                        Price = 1000,
                        Status = ProductStatus.Instock,
                        ProductId = 1
                    }
                }
            );
        }

        // example
        public async Task DeleteProduct()
        {
            var client = new ProductProtoService.ProductProtoServiceClient(Channel);

            var addProductRes = await client.DeleteProductAsync(
                new DeleteProductReq
                {
                    ProductId = 2
                }
            );
        }

        // example
        public async Task InsertBulkProduct()
        {
            var client = new ProductProtoService.ProductProtoServiceClient(Channel);

            for (int i = 0; i < 3; i++)
            {
                var productM = new ProductModel
                {
                    Description = "Des",
                    Name = $"Name{i}",
                    Price = 10000,
                    Status = ProductStatus.Instock
                };

                await client.InsertBulkProduct().RequestStream.WriteAsync(productM);
            }

            var res = await client.InsertBulkProduct().ResponseAsync;

            await client.InsertBulkProduct().RequestStream.CompleteAsync();

        }
    }
}
