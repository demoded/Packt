using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MusicStore.Models;
using MvcMusicStore.Helpers;
using MvcMusicStore.Models;

namespace MvcMusicStore.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        ApiHelper apiHelper;
        const string PromoCode = "FREE";


        public CheckoutController(IConfiguration _config)
        {
            apiHelper = new ApiHelper(_config.GetValue<string>("Services:MvcMusicStoreService"));
        }

       
      
        public ActionResult AddressAndPayment()
        {
            return View();
        }

      
        [HttpPost]
        public async Task<ActionResult> AddressAndPayment(IFormCollection values)
        {
            var order = new Order();
            bool success = TryUpdateModelAsync<Order>(order).Result;

            try
            {
                if (string.Equals(values["PromoCode"], PromoCode,
                    StringComparison.OrdinalIgnoreCase) == false)
                {
                    return View(order);
                }
                else
                {
                    order.Username = new ContextHelper().GetUsernameFromClaims(this.HttpContext);
                    order.OrderDate = DateTime.Now;

                    try
                    {
                        // get cart items
                        var cartId = new ShoppingCartHelper(this.HttpContext).GetCartId();
                        var cartItems = await apiHelper.GetAsync<List<Cart>>("/api/ShoppingCart/CartItems?id=" + cartId);
                        // avoid sending unuseful data on to the service
                        cartItems.ForEach((item) =>
                        {
                            item.Album.Genre = null;
                            item.Album.Artist = null;
                        });

                        OrderCreation creation = new OrderCreation()
                        {
                            OrderToCreate = order,
                            CartItems = cartItems
                        };
                        int orderId = await apiHelper.PostAsync<OrderCreation, int>("/api/Checkout/AddressAndPayment", creation);
                        await apiHelper.PostAsync<string>("/api/ShoppingCart/EmptyCart", $"'{cartId}'" );

                        return RedirectToAction("Complete", new { id = orderId });
                    }
                    catch (Exception)
                    {
                        //Log
                        return View(order);
                    }
                }

            }
            catch (Exception)
            {
                //Invalid - redisplay with errors
                return View(order);
            }
        }

        //
        // GET: /Checkout/Complete

        public async Task<ActionResult> Complete(int id)
        {
            try
            {
                string username = new ContextHelper().GetUsernameFromClaims(this.HttpContext);
                bool isValid = await apiHelper.GetAsync<bool>(string.Format("/api/Checkout/Complete?id={0}&username={1}", id, username));
                if (isValid)
                {
                    return View(id);
                }
                ViewBag.ErrorText = "Invalid checkout, order not found in the database.";
                return View("Error");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorText = ex.Message;
                return View("Error");
            }


        }
    }
}
