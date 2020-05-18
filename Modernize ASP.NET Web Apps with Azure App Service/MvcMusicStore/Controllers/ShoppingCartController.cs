using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MvcMusicStore.Helpers;
using MvcMusicStore.Models;
using MvcMusicStore.ViewModels;

namespace MvcMusicStore.Controllers
{
    public class ShoppingCartController : Controller
    {
        ApiHelper apiHelper;
        
        public ShoppingCartController(IConfiguration _config)
        {
            apiHelper = new ApiHelper(_config.GetValue<string>("Services:MvcMusicStoreService"));
        }

        //
        // GET: /ShoppingCart/

        public async Task<ActionResult> Index()
        {
            var cartId = new ShoppingCartHelper(this.HttpContext).GetCartId();
            // Set up our ViewModel
            var viewModel = new ShoppingCartViewModel
            {
                CartItems = await apiHelper.GetAsync<List<Cart>>("/api/ShoppingCart/CartItems?id=" + cartId),
                CartTotal = await apiHelper.GetAsync<decimal>("/api/ShoppingCart/Total?id=" + cartId)
            };

            // Return the view
            return View(viewModel);
        }

        //
        // GET: /Store/AddToCart/5

        public async Task<ActionResult> AddToCart(int id)
        {
            var cart = new Cart()
            {
                AlbumId = id,
                CartId = new ShoppingCartHelper(this.HttpContext).GetCartId()
            };
            await apiHelper.PostAsync<Cart>("/api/ShoppingCart/AddToCart", cart);
            // Go back to the main store page for more shopping
            return RedirectToAction("Index");
        }

        //
        // AJAX: /ShoppingCart/RemoveFromCart/5
        [HttpPost]
        public async Task<ActionResult> RemoveFromCart(int id)
        {
            var cartId = new ShoppingCartHelper(this.HttpContext).GetCartId();
            var cart = new Cart()
            {
                RecordId = id,
                CartId = cartId
            };
            
            int itemCount = await apiHelper.PostAsync<Cart, int>("/api/ShoppingCart/RemoveFromCart", cart);
            string albumName = await apiHelper.GetAsync<string>("/api/Store/AlbumName?id=" + id);

            // Display the confirmation message
            var results = new ShoppingCartRemoveViewModel
            {
                Message = HtmlEncoder.Default.Encode(albumName) +
                    " has been removed from your shopping cart.",
                CartTotal = await apiHelper.GetAsync<decimal>("/api/ShoppingCart/Total?id=" + cartId),
                CartCount = await apiHelper.GetAsync<int>("/api/ShoppingCart/Count?id=" + cartId),
                ItemCount = itemCount,
                DeleteId = id
            };

            return Json(results);
            //return RedirectToAction("Index");
        }


    }
}