using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MvcMusicStore.Helpers;
using MvcMusicStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcMusicStore.ViewComponents
{
    public class CartSummaryViewComponent : ViewComponent
    {
        ApiHelper apiHelper;

        public CartSummaryViewComponent(IConfiguration _config)
        {
            apiHelper = new ApiHelper(_config.GetValue<string>("Services:MvcMusicStoreService"));
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cart = new ShoppingCartHelper(this.HttpContext);
            var cartId = cart.GetCartId();
            int cartCount = await apiHelper.GetAsync<int>("/api/ShoppingCart/Count?id=" + cartId);
            if (this.HttpContext.User.Identity.IsAuthenticated)
            {
                string userName = new ContextHelper().GetUsernameFromClaims(this.HttpContext);

                int userCartCount = await apiHelper.GetAsync<int>("/api/ShoppingCart/Count?id=" + userName);
                if (userName != cartId && userCartCount == 0)
                {
                    CartMigration migration = new CartMigration()
                    {
                        SourceCartId = cartId,
                        DestCartId = userName
                    };
                    await apiHelper.PostAsync<CartMigration>("/api/ShoppingCart/MigrateCart", migration);
                    cartCount = userCartCount;
                }
                if (cartId != userName)
                {
                    cart.SetCartId(userName);
                    cartCount = await apiHelper.GetAsync<int>("/api/ShoppingCart/Count?id=" + userName);
                }
            }

            ViewData["CartCount"] = cartCount;
            return View();
        }
    }
}
