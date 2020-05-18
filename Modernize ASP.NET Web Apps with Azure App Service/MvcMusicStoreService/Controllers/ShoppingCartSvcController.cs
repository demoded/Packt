using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MvcMusicStore.Models;

namespace MvcMusicstoreService.Controllers
{
    [Route("api/ShoppingCart/[action]")]
    [ApiController]
    public class ShoppingCartSvcController : ControllerBase
    {
        MusicStoreEntities storeDB;

        public ShoppingCartSvcController(MusicStoreEntities _storeDB)
        {
            storeDB = _storeDB;
        }

        //
        // GET: /ShoppingCart/CartItems
        [HttpGet]
        public ActionResult<List<Cart>> CartItems(string id)
        {
            return new ShoppingCart(storeDB, id).GetCartItems();
        }

        // GET: /ShoppingCart/Total
        [HttpGet]
        public ActionResult<decimal> Total(string id)
        {
            return new ShoppingCart(storeDB, id).GetTotal();
        }


        // GET: /ShoppingCart/Count
        [HttpGet]
        public ActionResult<int> Count(string id)
        {
            return new ShoppingCart(storeDB, id).GetCount();
        }

        //
     

        [HttpPost]
        public ActionResult MigrateCart(CartMigration cartMigration)
        {
            new ShoppingCart(storeDB, cartMigration.SourceCartId).MigrateCart(cartMigration.DestCartId);
            return Ok();
        }

        // GET: /Store/AddToCart/5
        [HttpPost]
        public ActionResult AddToCart(Cart cart)
        {

            // Retrieve the album from the database
            var addedAlbum = storeDB.Albums
                .Single(album => album.AlbumId == cart.AlbumId);

            // Add it to the shopping cart
            new ShoppingCart(storeDB, cart.CartId).AddToCart(addedAlbum);

            // Go back to the main store page for more shopping
            return Ok();
        }

        //
        // AJAX: /ShoppingCart/RemoveFromCart/5
        [HttpPost]
        public ActionResult<int> RemoveFromCart(Cart cart)
        {
            // Remove the item from the cart
            int itemCount = new ShoppingCart(storeDB, cart.CartId).RemoveFromCart(cart.RecordId);
            return itemCount;
        }


        [HttpPost]
        public ActionResult EmptyCart([FromBody] string cartId)
        {
            new ShoppingCart(storeDB, cartId).EmptyCart();
            return Ok();
        }

    }
}