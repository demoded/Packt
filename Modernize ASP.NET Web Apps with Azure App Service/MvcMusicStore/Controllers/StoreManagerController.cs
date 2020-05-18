
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using MvcMusicStore.Helpers;
using MvcMusicStore.Models;
using Newtonsoft.Json;

namespace MvcMusicStore.Controllers
{
   // [Authorize(Roles = "Administrator")]
    public class StoreManagerController : Controller
    {

        ApiHelper apiHelper;
        IDistributedCache cache;

        public StoreManagerController(IConfiguration _config, IDistributedCache _distributedCache)
        {
            apiHelper = new ApiHelper(_config.GetValue<string>("Services:MvcMusicStoreService"));
            cache = _distributedCache;
        }
        //
        // GET: /StoreManager/

        public async Task<ViewResult> Index()
        {
            List<Album> albums = null;
            var albumsString = cache.GetString("albums") ?? "";
            if (string.IsNullOrEmpty(albumsString))
            {
                //Old code
                albums = await apiHelper.GetAsync<List<Album>>("/api/Store/Albums");

                var settings = new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects
                };
                albumsString = JsonConvert.SerializeObject(albums, settings);
                cache.SetString("albums", albumsString);
            }
            else
            {
                albums = JsonConvert.DeserializeObject<List<Album>>(albumsString);
            }
            return View(albums);
        }

        //
        // GET: /StoreManager/Details/5

        public async Task<ViewResult> Details(int id)
        {
            Album album = await apiHelper.GetAsync<Album>("/api/Store/Details/" + id);
            return View(album);
        }

        //
        // GET: /StoreManager/Create

        public async Task<ActionResult> Create()
        {
            var genres = await apiHelper.GetAsync<List<Genre>>("/api/Store/Genres");
            var artists = await apiHelper.GetAsync<List<Artist>>("/api/Store/Artists");

            ViewBag.GenreId = new SelectList(genres, "GenreId", "Name");
            ViewBag.ArtistId = new SelectList(artists, "ArtistId", "Name");
            return View();
        } 

        //
        // POST: /StoreManager/Create

        [HttpPost]
        public async Task<ActionResult> Create(Album album)
        {
            if (ModelState.IsValid)
            {
                await apiHelper.PostAsync<Album>("/api/Store/Create", album);
                cache.Remove("albums");
                return RedirectToAction("Index");  
            }

            var genres = await apiHelper.GetAsync<List<Genre>>("/api/Store/Genres");
            var artists = await apiHelper.GetAsync<List<Artist>>("/api/Store/Artists");

            ViewBag.GenreId = new SelectList(genres, "GenreId", "Name", album.GenreId);
            ViewBag.ArtistId = new SelectList(artists, "ArtistId", "Name", album.ArtistId);
            return View(album);
        }
        
        //
        // GET: /StoreManager/Edit/5
 
        public async Task<ActionResult> Edit(int id)
        {
            Album album = await apiHelper.GetAsync<Album>("/api/Store/Details/" + id);
            var genres = await apiHelper.GetAsync<List<Genre>>("/api/Store/Genres");
            var artists = await apiHelper.GetAsync<List<Artist>>("/api/Store/Artists");

            ViewBag.GenreId = new SelectList(genres, "GenreId", "Name", album.GenreId);
            ViewBag.ArtistId = new SelectList(artists, "ArtistId", "Name", album.ArtistId);
            return View(album);
        }

        //
        // POST: /StoreManager/Edit

        [HttpPost]
        public async Task<ActionResult> Edit(Album album)
        {
            if (ModelState.IsValid)
            {
                album.AlbumId = int.Parse(HttpContext.Request.Form["AlbumId"]);
                await apiHelper.PostAsync<Album>("/api/Store/Edit", album);
                cache.Remove("albums");
                return RedirectToAction("Index");
            }
            var genres = await apiHelper.GetAsync<List<Genre>>("/api/Store/Genres");
            var artists = await apiHelper.GetAsync<List<Artist>>("/api/Store/Artists");

            ViewBag.GenreId = new SelectList(genres, "GenreId", "Name", album.GenreId);
            ViewBag.ArtistId = new SelectList(artists, "ArtistId", "Name", album.ArtistId);
            return View(album);
        }

        //
        // GET: /StoreManager/Delete/5
 
        public async Task<ActionResult> Delete(int id)
        {
            Album album = await apiHelper.GetAsync<Album>("/api/Store/Details/" + id);
            return View(album);
        }

        //
        // POST: /StoreManager/Delete/5

        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            await apiHelper.DeleteAsync("/api/Store/Delete", id);
            cache.Remove("albums");
            return RedirectToAction("Index");
        }

       
    }
}