using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcMusicStore.Models
{
    public class CartMigration
    {
        public string SourceCartId { get; set; }
        public string DestCartId { get; set; }
    }
}
