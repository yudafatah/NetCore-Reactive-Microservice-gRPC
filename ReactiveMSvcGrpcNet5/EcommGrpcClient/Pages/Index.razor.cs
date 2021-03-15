using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EcommGrpcClient.Pages
{
    public partial class Index
    {
        private IList<ProductGrpc.Protos.ProductModel> productModels;

        protected override async Task OnInitializedAsync()
        {
            // for testing : waiting grpc server up
            System.Threading.Thread.Sleep(5000);

            var client = new ProductGrpc.Protos.ProductProtoService.ProductProtoServiceClient(Channel);

            // request param
            var getAllProducts = new ProductGrpc.Protos.GetAllProductReq();

            // get all product streaming data from grpc
            var resStream = client.GetAllProduct(getAllProducts);

            // add each stream to productModels
            List<ProductGrpc.Protos.ProductModel> temp = new List<ProductGrpc.Protos.ProductModel>();
            await foreach (var item in resStream.ResponseStream.ReadAllAsync())
            {
                temp.Add(item);

                Console.WriteLine(item);
            }

            productModels = temp;
        }
    }
}
