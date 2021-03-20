using AutoMapper;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShoppingCartGrpc.Models;
using ShoppingCartGrpc.Protos;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCartGrpc.Services
{
    [Authorize]
    public class ShoppingCartService : ShoppingCartProtoService.ShoppingCartProtoServiceBase
    {
        private readonly ShoppingCartContext shoppingCartContext;
        private readonly DiscountService discountService;
        private readonly ILogger<ShoppingCartService> logger;
        private readonly IMapper mapper;

        public ShoppingCartService(ShoppingCartContext shoppingCartContext, DiscountService discountService, 
            ILogger<ShoppingCartService> logger, IMapper mapper)
        {
            this.shoppingCartContext = shoppingCartContext ?? throw new ArgumentNullException(nameof(shoppingCartContext));
            this.discountService = discountService ?? throw new ArgumentNullException(nameof(discountService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override async Task<ShoppingCartModel> GetShoppingCart(GetShoppingCartReq request, ServerCallContext context)
        {
            var shoppingCart = await shoppingCartContext.ShoppingCart
                .FirstOrDefaultAsync(x => x.UserName == request.Username);

            if(shoppingCart == null)
            {
                logger.LogInformation($"ShoppingCart Cart with username = {request.Username} not found");
                throw new RpcException(new Status(StatusCode.NotFound, $"ShoppingCart Cart with username = {request.Username} not found"));
            }

            return mapper.Map<ShoppingCartModel>(shoppingCart);
        }

        public override async Task<ShoppingCartModel> CreateShoppingCart(ShoppingCartModel request, ServerCallContext context)
        {
            var shoppingCart = mapper.Map<ShoppingCart>(request);

            if (await shoppingCartContext.ShoppingCart.AnyAsync(x => x.UserName == shoppingCart.UserName))
            {
                logger.LogInformation($"Shopping cart with username {shoppingCart.UserName} already exist");
                throw new RpcException(new Status(StatusCode.NotFound, $"Shopping cart with username {shoppingCart.UserName} already exist"));
            }

            await shoppingCartContext.ShoppingCart.AddAsync(shoppingCart);
            await shoppingCartContext.SaveChangesAsync();

            logger.LogInformation("ShoppingCart cart created with username : {username}", shoppingCart.UserName);

            return mapper.Map<ShoppingCartModel>(shoppingCart);
        }

        public override async Task<RemoveItemIntoShoppingCartRes> RemoveItemIntoShoppingCart
            (RemoveItemIntoShoppingCartReq request, ServerCallContext context)
        {
            var shoppingCart = await shoppingCartContext.ShoppingCart
                .FirstOrDefaultAsync(x => x.UserName == request.Username);

            if(shoppingCart == null)
            {
                logger.LogInformation("Invalid username in removing shopping cart, username : {username}", request.Username);
                throw new RpcException(new Status(StatusCode.NotFound, $"Shopping Cart with username = {request.Username} not found"));
            }

            var removeCartItem = shoppingCart.items
                .FirstOrDefault(x => x.ProductId == request.RemoveCartItem.ProductId);

            if(removeCartItem == null)
            {
                logger.LogInformation("Invalid product Id in removing shopping cart item, Id : {Id}", request.RemoveCartItem.ProductId);
                throw new RpcException(new Status(StatusCode.NotFound, $"Shopping Cart Item with product Id = {request.RemoveCartItem.ProductId} not found"));
            }

            shoppingCart.items.Remove(removeCartItem);
            var removeCount = await shoppingCartContext.SaveChangesAsync();

            return new RemoveItemIntoShoppingCartRes { Success = removeCount > 0 };
        }

        public override async Task<AddItemIntoShoppingCartRes> AddItemIntoShoppingCart
            (IAsyncStreamReader<AddItemIntoShoppingCartReq> requestStream, ServerCallContext context)
        {
            await foreach (var item in requestStream.ReadAllAsync())
            {
                var shoppingCart = await shoppingCartContext.ShoppingCart
                    .FirstOrDefaultAsync(x => x.UserName == item.Username);
                if(shoppingCart == null)
                {
                    throw new RpcException(new Status(StatusCode.NotFound,""));
                }

                var cartItem = shoppingCart.items.FirstOrDefault(x => x.ProductId == item.NewCartItem.ProductId);

                if(cartItem == null)
                {
                    // grpc call discount service
                    var discount = await discountService.GetDiscount(item.DiscountCode);

                    item.NewCartItem.Price -= discount == null? 0 : discount.Amount;

                    shoppingCart.items.Add(mapper.Map<ShoppingCartItem>(item.NewCartItem));
                }
                else
                {
                    cartItem.Quantity += item.NewCartItem.Quantity;
                }
            }

            var insertCount = await shoppingCartContext.SaveChangesAsync();

            return new AddItemIntoShoppingCartRes { InsertCount = insertCount, Success = insertCount > 0 };
        }
    }
}
