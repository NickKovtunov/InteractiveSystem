using interactiveSystem.Models;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace interactiveSystem.Controllers
{
    public class HomeController : Controller
    {
        string dir = ConfigurationManager.AppSettings["PathToPhotoGallery"];
        string previewDir = ConfigurationManager.AppSettings["PathToPhotoGallery"] + @"\!Preview";
        [OutputCache(Duration = 21600)]
        public ActionResult Index()
        {
            var folders = System.IO.Directory.EnumerateDirectories(previewDir);
            var appUrl = HttpRuntime.AppDomainAppVirtualPath.TrimEnd('/');
            var galleries = folders
                .Select(fn => PhotoGallery.Create(fn))
                .Where(g => g != null)
                .OrderByDescending(g => g.OccurredOn).FirstOrDefault();

            ViewBag.LastFolderName = galleries.FolderName;
            ViewBag.LastFolderShortName = galleries.Title;
            ViewBag.FirstFileName = galleries.FirstFileName;


            var baseUrlImage = ConfigurationManager.AppSettings.Get("BaseUrlToTvNewsImage");
            var baseUrlVideo = ConfigurationManager.AppSettings.Get("BaseUrlToTvNewsVideo");
            using (var tvNews = new TVNewsContext())
            {
                var newsOrg = tvNews
                    .v_fullNews
                    .OrderByDescending(n => n.Date)
                    .FirstOrDefault();
                var news = new NewsDTO
                {
                    Id = newsOrg.Id,
                    Title = newsOrg.Title,
                    Description = newsOrg.Description,
                    Date = newsOrg.Date,
                    ImageUrl = $"{baseUrlImage}{newsOrg.ImageUrl}",
                    VideoUrl = $"{baseUrlVideo}{newsOrg.VideoUrl}",
                    //ImageUrl = $"{baseUrlImage}{n.ImageUrl}/preview.jpg",
                    //VideoUrl = $"{baseUrlVideo}{n.VideoUrl}/video.mp4"
                };
                ViewBag.VideoGalleryImg = news.ImageUrl;
            };



            //Вывод последней онлайн-новости
            using (var ctx = new ClientContext("http://portal.gd-urengoy.gazprom.ru"))
            {
                var web = ctx.Web;
                ctx.Load(web);

                var list = web.Lists.GetByTitle("Новости");
                ctx.Load(web.Lists);
                ctx.Load(list);

                var caml = new CamlQuery
                {
                    ViewXml =
                    @"<View>
                      <Query>
                        <ViewFields>
                          <FieldRef Name='Preview' />
                          <FieldRef Name='PublishingDate' />
                          <FieldRef Name='ImagePreview' />
                          <FieldRef Name='Title' />
                        </ViewFields>
                        <OrderBy>
                          <FieldRef Name='PublishingDate' Ascending='FALSE' />
                        </OrderBy>
                      </Query>
                        <RowLimit>1</RowLimit>
                    </View>"
                };
                var listItems = list.GetItems(caml);
                ctx.Load(listItems);
                ctx.ExecuteQuery();
                var res = new NewsGalleryListDTO
                {
                    Preview = listItems[0].FieldValues["Preview"],
                    PublishingDate = listItems[0].FieldValues["PublishingDate"],
                    ImagePreview = listItems[0].FieldValues["ImagePreview"],
                    Title = listItems[0].FieldValues["Title"]
                };

                if (res.Preview != null)
                {
                    res.Preview = res.Preview.ToString().Replace("src=\"", "src=\"http://portal.gd-urengoy.gazprom.ru");
                };
                if (res.ImagePreview != null)
                {
                    res.ImagePreview = res.ImagePreview.ToString().Replace("http://portal.gd-urengoy.gazprom.ru", "");
                    res.ImagePreview = res.ImagePreview.ToString().Replace("src=\"", "src=\"http://portal.gd-urengoy.gazprom.ru");
                    var resultSrc = $"src=\"{appUrl}/api/values/portal?url=${{ссылка}}\"";
                    res.ImagePreview = Regex.Replace(res.ImagePreview.ToString(), "src=[\"\'](?<ссылка>\\S+)[\"\']", resultSrc);
                }
                if (res.PublishingDate != null)
                {
                    res.PublishingDate = res.PublishingDate.ToString().Remove(10);
                }
                ViewBag.News = res;
            }

            ////Вывод последней офлайн-новости
            //using (ISContext db = new ISContext())
            //{
            //    NewsGallery newsGalleries = db.NewsGalleries.FirstOrDefault();

            //    if (newsGalleries != null)
            //    {
            //        var res = new NewsGalleryListDTO
            //        {
            //            Id = newsGalleries.Id,
            //            Preview = newsGalleries.Preview,
            //            PublishingDate = newsGalleries.PublishingDate,
            //            ImagePreview = newsGalleries.ImagePreview,
            //            Title = newsGalleries.Title
            //        };
            //        ViewBag.News = res;
            //    }
            //    else
            //    {
            //        ViewBag.News = newsGalleries;
            //    }
            //}


            return View();
        }

        public ActionResult History()
        {
            return View();
        }

        public ActionResult Portal()
        {
            return View();
        }

        public ActionResult Museum()
        {
            return View();
        }

        public ActionResult fiftyYears()
        {
            return View();
        }

        public ActionResult searchResults(string id)
        {
            ISContext db = new ISContext();
            IEnumerable<Leadership> searchLeaderships = db.Leaderships.Where(s => s.Name.ToString().Contains(id));
            ViewBag.searchLeaderships = searchLeaderships;
            IEnumerable<Man> searchMen = db.Men.Where(s => s.Name.ToString().Contains(id));
            ViewBag.searchMen = searchMen;
            IEnumerable<Enterprise> searchEnterprises = db.Enterprises.Where(s => s.Title.ToString().Contains(id));
            ViewBag.searchEnterprises = searchEnterprises;
            IEnumerable<Reward> searchRewards = db.Rewards.Where(s => s.Title.ToString().Contains(id));
            ViewBag.searchRewards = searchRewards;
            return View();
        }
    }
}
