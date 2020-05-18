using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MusicStore.Models;
using MvcMusicStore.Models;

namespace MvcMusicstoreService.Controllers
{
    [Route("api/Checkout/[action]")]
    [ApiController]
    public class CheckoutSvcController : ControllerBase
    {
        MusicStoreEntities storeDB;

        public CheckoutSvcController(MusicStoreEntities _storeDB)
        {
            storeDB = _storeDB;
        }


        [HttpPost]
        public ActionResult<int> AddressAndPayment(OrderCreation creation)
        {
            var order = creation.OrderToCreate;
                //Save Order
            storeDB.Orders.Add(order);
            storeDB.SaveChanges();

            //Process the order
            //shoppingCart.CreateOrder(order);
            decimal orderTotal = 0;

            
            // Iterate over the items in the cart, adding the order details for each
            foreach (var item in creation.CartItems)
            {
                var orderDetail = new OrderDetail
                {
                    AlbumId = item.AlbumId,
                    OrderId = order.OrderId,
                    UnitPrice = item.Album.Price,
                    Quantity = item.Count
                };

                // Set the order total of the shopping cart
                orderTotal += (item.Count * item.Album.Price);

                storeDB.OrderDetails.Add(orderDetail);

            }

            // Set the order's total to the orderTotal count
            order.Total = orderTotal;

            // Save the order
            storeDB.SaveChanges();
            
            return order.OrderId;
        }
        

        [HttpGet]
        public ActionResult<bool> Complete(int id, string username)
        {
            // Validate customer owns this order
            bool isValid = storeDB.Orders.Any(
                o => o.OrderId == id &&
                o.Username == username);

            return isValid;
        }
    }
}