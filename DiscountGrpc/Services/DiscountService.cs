using DiscountGrpc.Models;
using DiscountGrpc.Protos;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscountGrpc.Services
{
    public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
    {
        private readonly ILogger<DiscountService> logger;

        public DiscountService(ILogger<DiscountService> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override Task<DiscountModel> GetDiscount(GetDiscountReq request, ServerCallContext context)
        {
            var discount = DiscountContext.Discounts.FirstOrDefault(x => x.Code == request.DiscountCode);

            logger.LogInformation("Discount is used with discount code : {DiscountCode} and " +
                "the amount is : {discountAmount}", discount.Code, discount.Amount);

            return Task.FromResult(new DiscountModel
            {
                Amount = discount.Amount,
                Code = discount.Code,
                DiscountId = discount.DiscountId
            });
        }
    }
}
