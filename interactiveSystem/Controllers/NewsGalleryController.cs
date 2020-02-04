using interactiveSystem.Models;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;
using System.Text.RegularExpressions;
using System.Configuration;

namespace interactiveSystem.Controllers
{
    public class NewsGalleryController : Controller
    {
        //Вывод списка новостей для онлайн-версии
        [OutputCache(Duration = 86400, VaryByParam = "page")]
        public ActionResult Index(int? page)
        {
            var appUrl = HttpRuntime.AppDomainAppVirtualPath.TrimEnd('/');
            var res = new List<NewsGalleryListDTO>();
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
                    </View>"
                };
                var listItems = list.GetItems(caml);
                ctx.Load(listItems);
                ctx.ExecuteQuery();

                int i = listItems.Count - 1;
                foreach (var item in listItems)
                {
                    var publication = new NewsGalleryListDTO
                    {
                        Id = i,
                        Preview = item.FieldValues["Preview"],
                        PublishingDate = item.FieldValues["PublishingDate"],
                        ImagePreview = item.FieldValues["ImagePreview"],
                        Title = item.FieldValues["Title"]
                    };
                    res.Add(publication);
                    i--;
                }

                foreach (var n in res)
                {
                    if (n.Preview != null)
                    {
                        n.Preview = n.Preview.ToString().Replace("http://portal.gd-urengoy.gazprom.ru", "");
                        n.Preview = n.Preview.ToString().Replace("src=\"", "src=\"http://portal.gd-urengoy.gazprom.ru");
                    };
                    if (n.ImagePreview != null)
                    {
                        n.ImagePreview = n.ImagePreview.ToString().Replace("http://portal.gd-urengoy.gazprom.ru", "");
                        n.ImagePreview = n.ImagePreview.ToString().Replace("src=\"", "src=\"http://portal.gd-urengoy.gazprom.ru");
                        var resultSrc = $"src=\"{appUrl}/api/values/portal?url=${{ссылка}}\"";
                        n.ImagePreview = Regex.Replace(n.ImagePreview.ToString(), "src=[\"\'](?<ссылка>\\S+)[\"\']", resultSrc);
                    }
                    if (n.PublishingDate != null)
                    {
                        n.PublishingDate = n.PublishingDate.ToString().Remove(10);
                    }
                }

                int pageSize = 4;
                int pageNumber = (page ?? 1);
                IEnumerable<NewsGalleryListDTO> resultNewsList = res.ToPagedList(pageNumber, pageSize);
                ViewBag.resultNewsList = resultNewsList;
                return View(resultNewsList);
            }
        }

        //Вывод новости для онлайн-версии
        [OutputCache(Duration = 605000, VaryByParam = "id")]
        public ActionResult NewsDescription(int id)
        {
            var appUrl = HttpRuntime.AppDomainAppVirtualPath.TrimEnd('/');
            var res = new List<NewsGallerySingleDTO>();
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
                          <FieldRef Name='PublishingDate' />
                          <FieldRef Name='Title' />
                          <FieldRef Name='Text' />
                        </ViewFields>
                        <OrderBy>
                          <FieldRef Name='PublishingDate' Ascending='TRUE' />
                        </OrderBy>
                      </Query>
                    </View>"
                };
                var listItems = list.GetItems(caml);
                ctx.Load(listItems);
                ctx.ExecuteQuery();

                var newsDescription = new NewsGallerySingleDTO
                {
                    PublishingDate = listItems[id].FieldValues["PublishingDate"],
                    Title = listItems[id].FieldValues["Title"],
                    Text = listItems[id].FieldValues["Text"],
                };

                if (newsDescription.Text != null)
                {
                    newsDescription.Text = Regex.Replace(newsDescription.Text.ToString(), "style=[\"\'].*?[\"\']", string.Empty);
                    newsDescription.Text = Regex.Replace(newsDescription.Text.ToString(), "<a.*?>", string.Empty);
                    newsDescription.Text = Regex.Replace(newsDescription.Text.ToString(), "<//a>", string.Empty);
                    newsDescription.Text = newsDescription.Text.ToString().Replace("http://portal.gd-urengoy.gazprom.ru", "");
                    newsDescription.Text = newsDescription.Text.ToString().Replace("src=\"", "src=\"http://portal.gd-urengoy.gazprom.ru");
                    var resultSrc = $"src=\"{appUrl}/api/values/portal?url=${{ссылка}}\"";
                    newsDescription.Text = Regex.Replace(newsDescription.Text.ToString(), "src=[\"\'](?<ссылка>\\S+)[\"\']", resultSrc);
                };

                if (newsDescription.PublishingDate != null)
                {
                    newsDescription.PublishingDate = newsDescription.PublishingDate.ToString().Remove(10);
                };

                ViewBag.newsDescription = newsDescription;
                return View();
            }
        }

        string dir = ConfigurationManager.AppSettings["PathToNewsGallery"];
        //Синхронизация БД ленты новостей для офлайн-версии
        ISContext db = new ISContext();
        [Authorize(Roles = "GD-URENGOY\\editors-IS")]
        public ActionResult NewsSync()
        {
            IEnumerable<NewsGallery> newsGalleries = db.NewsGalleries;
            foreach (var os in newsGalleries)
            {
                db.NewsGalleries.Remove(os);
            }
            db.SaveChanges();

            var res = new List<NewsGallery>();
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
                    </View>"
                };
                var listItems = list.GetItems(caml);
                ctx.Load(listItems);
                ctx.ExecuteQuery();

                int i = listItems.Count - 1;
                foreach (var item in listItems)
                {
                    string Preview = null;
                    string PublishingDate = null;
                    string ImagePreview = null;
                    string Title = null;
                    string Text = null;

                    if (item.FieldValues["Preview"] != null)
                    {
                        Preview = item.FieldValues["Preview"].ToString().Replace("http://portal.gd-urengoy.gazprom.ru", "");
                        Preview = Preview.Replace("src=\"", "src=\"" + dir);
                    };

                    if (item.FieldValues["PublishingDate"] != null)
                    {
                        PublishingDate = item.FieldValues["PublishingDate"].ToString().Remove(10);
                    };

                    if (item.FieldValues["ImagePreview"] != null)
                    {
                        ImagePreview = item.FieldValues["ImagePreview"].ToString().Replace("http://portal.gd-urengoy.gazprom.ru", "");
                        ImagePreview = ImagePreview.Replace("src=\"", "src=\"" + dir);
                    };

                    if (item.FieldValues["Title"] != null)
                    {
                        Title = item.FieldValues["Title"].ToString();
                    };

                    if (item.FieldValues["Text"] != null)
                    {
                        Text = item.FieldValues["Text"].ToString();
                        Text = Regex.Replace(Text, "style=[\"\'].*?[\"\']", string.Empty);
                        Text = Regex.Replace(Text, "<a.*?>", string.Empty);
                        Text = Regex.Replace(Text, "<//a>", string.Empty);
                        Text = Text.Replace("http://portal.gd-urengoy.gazprom.ru", "");
                        Text = Text.Replace("src=\"", "src=\"" + dir);
                    };

                    var publication = new NewsGallery
                    {
                        Id = i,
                        Preview = Preview,
                        PublishingDate = PublishingDate,
                        ImagePreview = ImagePreview,
                        Title = Title,
                        Text = Text,
                    };
                    res.Add(publication);
                    i--;
                }

                foreach (var n in res)
                {
                    db.NewsGalleries.Add(n);
                    db.SaveChanges();
                }
            }

            return View();
        }

        ////Вывод списка новостей для офлайн-версии
        //public ActionResult Index(int? page)
        //{
        //    IEnumerable<NewsGallery> newsGalleries = db.NewsGalleries;
        //    var res = new List<NewsGalleryListDTO>();

        //    foreach (var item in newsGalleries)
        //    {
        //        var publication = new NewsGalleryListDTO
        //        {
        //            Id = item.Id,
        //            Preview = item.Preview,
        //            PublishingDate = item.PublishingDate,
        //            ImagePreview = item.ImagePreview,
        //            Title = item.Title
        //        };
        //        res.Add(publication);
        //    }

        //    int pageSize = 4;
        //    int pageNumber = (page ?? 1);
        //    IEnumerable<NewsGalleryListDTO> resultNewsList = res.ToPagedList(pageNumber, pageSize);
        //    ViewBag.resultNewsList = resultNewsList;
        //    return View(resultNewsList);
        //}

        ////Вывод новости для офлайн-версии
        //public ActionResult NewsDescription(int id)
        //{
        //    NewsGallery newsGallery = db.NewsGalleries.Find(id);
        //    var res = new List<NewsGallerySingleDTO>();

        //    var newsDescription = new NewsGallerySingleDTO
        //    {
        //        PublishingDate = newsGallery.PublishingDate,
        //        Title = newsGallery.Title,
        //        Text = newsGallery.Text,
        //    };

        //    ViewBag.newsDescription = newsDescription;
        //    return View();
        //}
    }
}