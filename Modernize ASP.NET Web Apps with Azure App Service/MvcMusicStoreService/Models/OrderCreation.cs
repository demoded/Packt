using MvcMusicStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicStore.Models
{
    public class OrderCreation
    {
        public Order OrderToCreate { get; set; }
        public List<Cart> CartItems { get; set; }
    }
}
