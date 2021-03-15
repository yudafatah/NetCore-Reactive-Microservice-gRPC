using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductGrpc.Models;
using ProductGrpc.Protos;
using System;
using System.Threading.Tasks;
using static ProductGrpc.CQRS.Queries.ProductQuery;

namespace ProductGrpc.Services
{
    public class ProductService : ProductProtoService.ProductProtoServiceBase
    {
        private readonly ProductContext productContext;
        private readonly ILogger<ProductService> logger;
        private readonly IMapper mapper;
        private readonly IMediator mediator;

        public ProductService(ProductContext productContext, ILogger<ProductService> logger, 
            IMapper mapper, IMediator mediator)
        {
            this.productContext = productContext ?? throw new ArgumentNullException(nameof(productContext));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override Task<Empty> Test(Empty req, ServerCallContext context)
        {
            return base.Test(req, context);
        }

        public override async Task<ProductModel> GetProduct(GetProductReq request, ServerCallContext context)
        {
            //var product = await productContext.Product.FindAsync(request.ProductId);
            var commandReq = mapper.Map<GetProductQuery>(request);

            var product = await mediator.Send(commandReq);

            if(product == null)
            {
                throw new RpcException(new Status(StatusCode.OK, "data empty"));
            }

            return await Task.FromResult(mapper.Map<ProductModel>(product));
        }

        public override async Task GetAllProduct(GetAllProductReq request, 
            IServerStreamWriter<ProductModel> responseStream, ServerCallContext context)
        {
            var products = await productContext.Product.ToListAsync();

            foreach (var product in products)
            {
                await responseStream.WriteAsync(mapper.Map<ProductModel>(product));
            }
        }

        public override async Task<ProductModel> AddProduct(AddProductReq request, ServerCallContext context)
        {
            await productContext.Product.AddAsync(mapper.Map<Product>(request.Product));
            await productContext.SaveChangesAsync();

            return await Task.FromResult(request.Product);
        }
    }
}
