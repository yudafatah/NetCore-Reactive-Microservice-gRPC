using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using ProductGrpc.Models;
using ProductGrpc.Protos;
using static ProductGrpc.CQRS.Queries.ProductQuery;

namespace ProductGrpc.Mapper
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductModel>()
                .ForMember(x => x.CreatedTime, opt => opt.MapFrom(src => Timestamp.FromDateTime(src.CreatedTime)))
                .ForMember(x => x.Status, opt => opt.MapFrom(src => (Protos.ProductStatus)src.Status));

            CreateMap<ProductModel, Product>()
                .ForMember(x => x.CreatedTime, opt => opt.MapFrom(src => src.CreatedTime.ToDateTime()))
                .ForMember(x => x.Status, opt => opt.MapFrom(src => (Models.ProductStatus)src.Status));

            CreateMap<GetProductQuery, GetProductReq>();
            CreateMap<GetProductReq, GetProductQuery>();
        }
    }
}
