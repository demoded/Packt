using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcMusicStore.Helpers
{
    public class ContextHelper
    {

        public string GetUsernameFromClaims(HttpContext context)
        {
            return context.User.Claims.Where(c => c.Type == "emails").FirstOrDefault().Value;
        }
    }
}
