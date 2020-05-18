using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MvcMusicStore.Helpers;
using MvcMusicStore.Models;

namespace MvcMusicStore.Controllers
{

    public class StoreController : Controller
    {
        ApiHelper apiHelper;

        public StoreController(IConfiguration _config)
        {
            apiHelper = new ApiHelper(_config.GetValue<string>("Services:MvcMusicStoreService"));
        }
        //
        // GET: /Store/

        public async Task<ActionResult> Index()
        {
            var genres = await apiHelper.GetAsync<List<Genre>>("/api/Store/Genres");
            return View(genres);
        }

        //
        // GET: /Store/Browse?genre=Disco

        public async Task<ActionResult> Browse(string genre)
        {
            // Retrieve Genre and its Associated Albums from database
            var genreModel = await apiHelper.GetAsync<Genre>("/api/Store/Browse?genre=" + genre);
            return View(genreModel);
        }

        //
        // GET: /Store/Details/5

        public async Task<ActionResult> Details(int id)
        {
            var album = await apiHelper.GetAsync<Album>("/api/Store/Details/" + id);
            return View(album);
        }

    }
}