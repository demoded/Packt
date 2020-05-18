using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MvcMusicStore.Helpers;
using MvcMusicStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcMusicStore.ViewComponents
{
    public class GenreMenu : ViewComponent
    {
        ApiHelper apiHelper;

        public GenreMenu(IConfiguration _config)
        {
            apiHelper = new ApiHelper(_config.GetValue<string>("Services:MvcMusicStoreService"));
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var genres = await apiHelper.GetAsync<List<Genre>>("/api/Store/Genres");
            return View(genres);
        }
    }
}
