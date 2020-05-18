using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMusicStore.Models;
using Newtonsoft.Json;

namespace MvcMusicstoreService.Controllers
{
    [Route("api/Store/[action]/{id?}")]
    [ApiController]
    public class StoreSvcController : ControllerBase
    {
        MusicStoreEntities storeDB;

        public StoreSvcController(MusicStoreEntities _storeDB)
        {
            storeDB = _storeDB;
        }
        //
        // GET: /Store/


        [HttpGet]
        public ActionResult<List<Album>> Albums()
        {
            var v = storeDB.Albums.Include(a => a.Genre).Include(a => a.Artist).ToList();
            return v;
        }

        [HttpGet]
        public ActionResult<List<Genre>> Genres()
        {
            var genres = storeDB.Genres;
            return genres.ToList();
        }   

        //
        // GET: /Store/Browse?genre=Disco

        [HttpGet]
        public ActionResult<Genre> Browse(string genre)
        {
            // Retrieve Genre and its Associated Albums from database
            return storeDB.Genres.Include("Albums")
                .Single(g => g.Name == genre);
        }

        [HttpGet]
        public ActionResult<List<Artist>> Artists()
        {
            // Retrieve Genre and its Associated Albums from database
            return storeDB.Artists.ToList();

        }

        //
        // GET: /Store/Details/5
        [HttpGet]
        public ActionResult<Album> Details(int id)
        {
            return storeDB.Albums.Where(a => a.AlbumId == id).FirstOrDefault();
        }

        [HttpGet]
        public ActionResult<List<Album>> TopSellingAlbums(int count)
        {
            // Group the order details by album and return
            // the albums with the highest count

            var v =  storeDB.Albums
                .OrderByDescending(a => a.OrderDetails.Count())
                .Take(count)
                .ToList();
            return v;
        }


        [HttpGet]
        public ActionResult<string> AlbumName(int id)
        {
            //// Get the name of the album to display confirmation
            return storeDB.Carts
                .Single(item => item.RecordId == id).Album.Title;
        }




        [HttpPost]
        public ActionResult Create(Album album)
        {
            storeDB.Albums.Add(album);
            storeDB.SaveChanges();
            return Ok();
        }

        [HttpPost]
        public ActionResult Edit(Album album)
        {
            storeDB.Entry(album).State = EntityState.Modified;
            storeDB.SaveChanges();
            return Ok();
        }


        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Album album = storeDB.Albums.Find(id);
            storeDB.Albums.Remove(album);
            storeDB.SaveChanges();
            return Ok();
        }

    }
}