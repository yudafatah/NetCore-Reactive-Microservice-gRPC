using DiscountGrpc.Protos;
using System;
using System.Threading.Tasks;

namespace ShoppingCartGrpc.Services
{
    public class DiscountService
    {
        private readonly DiscountProtoService.DiscountProtoServiceClient discountProtoService;

        public DiscountService(DiscountProtoService.DiscountProtoServiceClient discountProtoService)
        {
            this.discountProtoService = discountProtoService ?? throw new ArgumentNullException(nameof(discountProtoService));
        }

        public async Task<DiscountModel> GetDiscount(string discountCode)
        {
            var discountReq = new GetDiscountReq { DiscountCode = discountCode };
            return await discountProtoService.GetDiscountAsync(discountReq);
        }
    }
}
