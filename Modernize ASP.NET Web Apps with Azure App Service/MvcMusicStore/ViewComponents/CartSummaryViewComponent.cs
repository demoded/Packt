using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MvcMusicStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcMusicStore.ViewComponents
{
    public class CartSummaryViewComponent : ViewComponent
    {
        private readonly ShoppingCart cart;
        
        public CartSummaryViewComponent(MusicStoreEntities _storeDB, IHttpContextAccessor _httpContextAccessor)
        {
            cart = ShoppingCart.GetCart(_httpContextAccessor.HttpContext, _storeDB);
        }

        public IViewComponentResult Invoke()
        {
            ViewData["CartCount"] = cart.GetCount();
            return View();
        }
    }
}
