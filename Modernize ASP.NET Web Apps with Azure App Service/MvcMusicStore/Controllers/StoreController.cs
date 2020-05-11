using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMusicStore.Models;

namespace MvcMusicStore.Controllers
{
    public class StoreController : Controller
    {
        MusicStoreEntities storeDB;

        public StoreController(MusicStoreEntities _storeDB)
        {
            storeDB = _storeDB;
        }
        //
        // GET: /Store/

        public ActionResult Index()
        {
            var genres = storeDB.Genres.ToList();

            return View(genres);
        }

        //
        // GET: /Store/Browse?genre=Disco

        public ActionResult Browse(string genre)
        {
            // Retrieve Genre and its Associated Albums from database
            var genreModel = storeDB.Genres.Include("Albums")
                .Single(g => g.Name == genre);

            return View(genreModel);
        }

        //
        // GET: /Store/Details/5

        public ActionResult Details(int id)
        {
            var album = storeDB.Albums.Where(a => a.AlbumId == id).FirstOrDefault();

            return View(album);
        }

        //
        // GET: /Store/GenreMenu
        //// [ChildActionOnly] // MIGRATION
        //public ActionResult GenreMenu()
        //{
        //    var genres = storeDB.Genres.ToList();

        //    return PartialView(genres);
        //}


    }
}