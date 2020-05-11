using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MvcMusicStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcMusicStore.ViewComponents
{
    public class GenreMenu : ViewComponent
    {
        private readonly MusicStoreEntities storeDB;
        
        public GenreMenu(MusicStoreEntities _storeDB)
        {
            storeDB = _storeDB;
        }

        public IViewComponentResult Invoke()
        {
            var genres = storeDB.Genres.ToList();
            return View(genres);
        }
    }
}
