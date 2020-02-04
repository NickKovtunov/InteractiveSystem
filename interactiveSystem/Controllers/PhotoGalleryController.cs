using HtmlAgilityPack;
using interactiveSystem.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImageResizer;
using XperiCode.JpegMetadata;
using System.Configuration;

namespace interactiveSystem.Controllers
{
    public class PhotoGalleryController : Controller
    {
        ISContext db = new ISContext();
        string dir = ConfigurationManager.AppSettings["PathToPhotoGallery"];
        string previewDir = ConfigurationManager.AppSettings["PathToPhotoGallery"] + @"\!Preview\";
        public ActionResult Index()
        {
            var folders = System.IO.Directory.EnumerateDirectories(previewDir);

            var galleries = folders
                .Select(fn => PhotoGallery.Create(fn))
                .Where(g => g != null)
                .OrderByDescending(g => g.OccurredOn);

            //foreach (var b in galleries)
            //{
            //    string dirName = dir + @"\" + b.OccurredOnForLink + " " + b.Title;
            //    if (Directory.Exists(dirName))
            //    {
            //        b.FirstFileName = Path.GetFileName(Directory.GetFiles(dirName).FirstOrDefault());
            //    }
            //}

            ViewBag.Response = galleries;


            //string dirName = "\\\\gd-urengoy\\ГДУ\\Фотогалерея";
            //if (Directory.Exists(dirName))
            //{
            //    string[] dirs = Directory.GetDirectories(dirName);
            //    ViewBag.Response = dirs;
            //}

            //var url = "http://www.ugp.ru/press/gallery/";
            //var web = new HtmlWeb();
            //var doc = web.Load(url);//, "GET", proxy, CredentialCache.DefaultNetworkCredentials);
            //var nodes = doc.DocumentNode.Descendants("div")
            //    .Select(y => y)
            //    .Where(x => x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("photo_gallery_cont"))
            //    .ToList();
            //var resultString = nodes[0].InnerHtml.ToString().Replace("src=\"", "src=\"http://www.ugp.ru");
            //resultString = resultString.Replace("href=\"/press/gallery/", "href=\"/PhotoGallery/Album/");
            return View();
        }

        //Просмотр альбома
        [HttpGet]
        public ActionResult Album(string id)
        {
            //var directories = ImageMetadataReader.ReadMetadata(@"D:\Projects\interactiveSystem\interactiveSystem\Content\images\anons");
            //Image testimage = new Bitmap(@"D:\Projects\interactiveSystem\interactiveSystem\Content\images\anons.jpg");
            //PropertyItem[] propItems = testimage.PropertyItems;
            //var adapter = new JpegMetadataAdapter(@"D:\Projects\interactiveSystem\interactiveSystem\Content\images\anons.jpg").Metadata.Title;


            string previewDirName = previewDir + @"\" + id;
            string dirName = dir + @"\" + id;
            if (Directory.Exists(previewDirName))
            {
                string[] files = Directory.GetFiles(previewDirName);
                var image = files
                    .Select(fn => new ImageDTO { FileName = Path.GetFileName(fn), FolderName = Path.GetFileName(previewDirName), FileDescription = new JpegMetadataAdapter(dirName + @"\" + Path.GetFileName(fn)).Metadata.Title })
                    //.Select(i => this.Url.Link("Default", new { Controller = "MyMvc", Action = "MyAction", param1 = 1, param2 = "somestring" });)
                    .OrderBy(g => g.FileName)
                    .ToList();
                ViewBag.Response = image;
                ViewBag.FolderTitle = id;
            }
            return View();
        }

        //Синхронизация
        [HttpGet]
        public ActionResult Synchronization()
        {
            //Очищаем таблицу Man-Photos (type = image) от старых значений
            IEnumerable<Man_Photo> manPhotos = db.Man_Photos.Where(s => s.GalleryType == "image");
            foreach (var os in manPhotos)
            {
                db.Man_Photos.Remove(os);
            }
            db.SaveChanges();

            if (!Directory.Exists(previewDir))
            {
                Directory.CreateDirectory(previewDir);
            };

            var folders = System.IO.Directory.EnumerateDirectories(dir);
            foreach (var b in folders)
            {
                var FullFolderName = b.ToString();
                var FolderName = FullFolderName.Replace(dir, "");
                var PreviewFullFolderName = previewDir + @"\" + FolderName;
                if (FolderName != "!Preview")
                {
                    if (!Directory.Exists(PreviewFullFolderName))
                    {
                        Directory.CreateDirectory(PreviewFullFolderName);
                    };

                    string[] files = Directory.GetFiles(FullFolderName);
                    foreach (var f in files)
                    {
                        var FullFileName = f.ToString();
                        var FileName = FullFileName.Replace(FullFolderName + @"\", "");
                        var PreviewFullFileName = previewDir + @"\" + FolderName + @"\" + FileName;
                        FileInfo previewFileInf = new FileInfo(PreviewFullFileName);
                        FileInfo fullFileInf = new FileInfo(FullFileName);
                        var ext = fullFileInf.Extension.ToLower();
                        if (!previewFileInf.Exists&&(ext == ".jpg"))
                        {
                            ImageBuilder.Current.Build(
                                new ImageJob(
                                    f,
                                    PreviewFullFileName,
                                    new Instructions("maxwidth=975&maxheight=300"),
                                    false,
                                    false));
                        };

                        //запись txt-файлов в базу
                        if (ext == ".txt")
                        {

                            using (FileStream fstream = new FileStream(FullFileName, FileMode.Open))
                            {
                                // преобразуем строку в байты
                                byte[] array = new byte[fstream.Length];
                                // считываем данные
                                fstream.Read(array, 0, array.Length);
                                // декодируем байты в строку
                                string textFromFile = System.Text.Encoding.Default.GetString(array);
                                string[] textFromFileArray = textFromFile.Split(new char[] { ';' });

                                foreach (string s in textFromFileArray)
                                {
                                    var newS = s.Replace(@"\r\n", "").Trim();
                                    IEnumerable<Man> photoMen = db.Men.Where(r => r.Name.Equals(newS));
                                    foreach (var m in photoMen)
                                    {
                                        Man_Photo mp = new Man_Photo();
                                        mp.UserId = m.Id;
                                        mp.GalleryType = "image";
                                        mp.FolderName = FolderName;
                                        mp.OrgFolderName = "";
                                        mp.FileName = FileName.Replace(".txt", ".jpg");
                                        var metaFile = FullFileName.Replace(".txt", ".jpg");
                                        mp.FileDescription = new JpegMetadataAdapter(metaFile).Metadata.Title;
                                        db.Man_Photos.Add(mp);
                                    }
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }

            //string dirName = dir + @"\" + id;
            //if (Directory.Exists(dirName))
            //{
            //    string[] files = Directory.GetFiles(dirName);
            //    var image = files
            //        .Select(fn => new ImageDTO { FileName = Path.GetFileName(fn), FolderName = Path.GetFileName(dirName) })
            //        //.Select(i => this.Url.Link("Default", new { Controller = "MyMvc", Action = "MyAction", param1 = 1, param2 = "somestring" });)
            //        .ToList();
            //    ViewBag.Response = image;
            //    ViewBag.FolderTitle = id;
            //}
            return View();
        }


    }
}