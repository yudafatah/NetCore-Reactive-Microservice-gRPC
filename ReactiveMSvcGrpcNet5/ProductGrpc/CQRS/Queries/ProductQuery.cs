using MediatR;
using ProductGrpc.Models;

namespace ProductGrpc.CQRS.Queries
{
    public class ProductQuery
    {
        /// <summary>
        /// Interface that represents a query to the system
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public interface IQuery<T> : IRequest<T>
        {
        }

        /// <summary>
        /// Get Product by request
        /// </summary>
        /// <remarks>
        /// Send this query command to get product by request in the system
        /// </remarks>
        public record GetProductQuery : IQuery<Product>
        {
            /// <summary>
            /// The product Id
            /// </summary>
            /// <example>1234</example>
            public int ProductId { get; set; }
        }
    }
}
