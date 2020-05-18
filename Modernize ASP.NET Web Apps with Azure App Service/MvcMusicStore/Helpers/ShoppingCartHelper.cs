using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcMusicStore.Helpers
{
    public class ShoppingCartHelper
    {
        const string CartSessionKey = "CartId";
        HttpContext context;

        public ShoppingCartHelper(HttpContext _context)
        {
            context = _context;
        }

        public string GetCartId()
        {
            if (!context.Session.IsAvailable)
            {
                context.Session.LoadAsync().Wait();
            }
            if (context.Session.GetString(CartSessionKey) == null)
            {
                                // Generate a new random GUID using System.Guid class
                Guid tempCartId = Guid.NewGuid();

                // Send tempCartId back to client as a cookie
                context.Session.SetString(CartSessionKey, tempCartId.ToString());
                context.Session.CommitAsync().Wait();
            }

            return context.Session.GetString(CartSessionKey);
        }

        public void SetCartId(string cartId)
        {
            context.Session.LoadAsync().Wait();
            context.Session.SetString(CartSessionKey, cartId);
            context.Session.CommitAsync().Wait();
        }
    }
}
