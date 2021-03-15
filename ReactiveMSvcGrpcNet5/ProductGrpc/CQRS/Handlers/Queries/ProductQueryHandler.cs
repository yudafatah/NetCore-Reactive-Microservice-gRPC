using MediatR;
using ProductGrpc.Models;
using System.Threading;
using System.Threading.Tasks;
using static ProductGrpc.CQRS.Queries.ProductQuery;

namespace ProductGrpc.CQRS.Handlers.Queries
{
    public class ProductQueryHandler : IRequestHandler<GetProductQuery, Product>
    {
        private readonly ProductContext productContext;

        public ProductQueryHandler(ProductContext productContext)
        {
            this.productContext = productContext;
        }

        public async Task<Product> Handle(GetProductQuery request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(await productContext.Product.FindAsync(request.ProductId));
        }
    }
}
