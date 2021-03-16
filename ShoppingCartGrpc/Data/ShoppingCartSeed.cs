using ShoppingCartGrpc.Models;
using System.Collections.Generic;
using System.Linq;

namespace ShoppingCartGrpc.Data
{
    public class ShoppingCartSeed
    {
        public static void SeedAsync(ShoppingCartContext shoppingCartContext)
        {
            if (!shoppingCartContext.ShoppingCart.Any())
            {
                var shoppingCarts = new List<ShoppingCart>
                {
                    new ShoppingCart
                    {
                        UserName = "test",
                        items = new List<ShoppingCartItem>
                        {
                            new ShoppingCartItem
                            {
                                Color = "red",
                                Price = 10000,
                                ProductId = 1,
                                ProductName = "name",
                                Quantity = 11
                            },
                            new ShoppingCartItem
                            {
                                Color = "red",
                                Price = 10000,
                                ProductId = 2,
                                ProductName = "name2",
                                Quantity = 11
                            }
                        }
                    }
                };

                shoppingCartContext.ShoppingCart.AddRange(shoppingCarts);
                shoppingCartContext.SaveChanges();
            }
        }
    }
}
