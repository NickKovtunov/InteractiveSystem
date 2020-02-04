using interactiveSystem.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;

namespace interactiveSystem.Controllers
{
    public class VideoGalleryController : Controller
    {

        public ActionResult Index(int? page, string searchString)
        {
            var baseUrlImage = ConfigurationManager.AppSettings.Get("BaseUrlToTvNewsImage");
            var baseUrlVideo = ConfigurationManager.AppSettings.Get("BaseUrlToTvNewsVideo");
            using (var tvNews = new TVNewsContext())
            {
                var news = tvNews
                    .v_fullNews
                    .OrderByDescending(n => n.Date)
                    .ToList()
                    .Select(n => new NewsDTO
                    {
                        Id = n.Id,
                        Title = n.Title,
                        Description = n.Description,
                        Date = n.Date,
                        ImageUrl = $"{baseUrlImage}{n.ImageUrl}",
                        VideoUrl = $"{baseUrlVideo}{n.VideoUrl}",
                        //ImageUrl = $"{baseUrlImage}{n.ImageUrl}/preview.jpg",
                        //VideoUrl = $"{baseUrlVideo}{n.VideoUrl}/video.mp4"
                    });
                int pageSize = 6;
                int pageNumber = (page ?? 1);
                ViewBag.newsQuantity = 0;
                ViewBag.NewsDate = "Поиск выпуска по дате";

                if (!String.IsNullOrEmpty(searchString))
                {
                    news = news.Where(s => s.Date.ToString().Contains(searchString));
                    ViewBag.NewsDate = searchString;
                }

                IEnumerable<NewsDTO> newsList = news.ToPagedList(pageNumber, pageSize);

                ViewBag.NewsList = newsList;
                ViewBag.newsQuantity = newsList.Count();
                return View(newsList);
            }
        }
    }
}