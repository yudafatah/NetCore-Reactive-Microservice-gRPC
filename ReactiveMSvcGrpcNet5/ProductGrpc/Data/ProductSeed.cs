using ProductGrpc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductGrpc.Data
{
    public class ProductSeed
    {
        public static void SeedAsync(ProductContext productContext)
        {
            if (!productContext.Product.Any())
            {
                var products = new List<Product>();

                for (int i = 1; i < 101; i++)
                {
                    products.Add(new Product
                    {
                        CreatedTime = DateTime.UtcNow,
                        Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum",
                        Name = "Product Name",
                        Price = 100000,
                        ProductId = i,
                        Status = ProductStatus.INSTOCK
                    });
                }

                productContext.Product.AddRange(products);
                productContext.SaveChanges();
            }
        }
    }
}
