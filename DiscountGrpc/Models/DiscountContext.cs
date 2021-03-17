using System.Collections.Generic;

namespace DiscountGrpc.Models
{
    public class DiscountContext
    {
        public static readonly List<Discount> Discounts = new List<Discount>
        {
            new Discount{DiscountId=1,Code="Code1",Amount=100},
            new Discount{DiscountId=2,Code="Code2",Amount=400},
            new Discount{DiscountId=3,Code="Code3",Amount=200}
        };
    }
}
