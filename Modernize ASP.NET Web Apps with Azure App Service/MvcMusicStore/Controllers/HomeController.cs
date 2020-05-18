using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MvcMusicStore.Helpers;
using MvcMusicStore.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MvcMusicStore.Controllers
{
    public class HomeController : Controller
    {
        ApiHelper apiHelper;

        public HomeController(IConfiguration _config)
        {
            apiHelper = new ApiHelper(_config.GetValue<string>("Services:MvcMusicStoreService"));
        }
        public async Task<ActionResult> Index()
        {

            // Get most popular albums
            var albums = await apiHelper.GetAsync<List<Album>>("/api/Store/TopSellingAlbums?count=5");
            return View(albums);
        }

        
    }
}
