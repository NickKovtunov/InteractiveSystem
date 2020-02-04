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
using System.Text.RegularExpressions;

namespace interactiveSystem.Controllers
{
    public class BooksController : Controller
    {
        ISContext db = new ISContext();
        string dir = ConfigurationManager.AppSettings["PathToBooks"];

        //Просмотр альбома
        [HttpGet]
        public ActionResult Album(string folder,string id)
        {
            ViewBag.OrgId = folder;
            ViewBag.FolderTitle = id;

            var mth = Regex.Match(id, @"(?<date>\d{2}\-\d{2}\-\d{4})\s+(?<title>.+)");
            if (mth.Success)
            {
                ViewBag.ShortFolderTitle = mth.Groups["title"].Value;
            }

            string previewDirName = ConfigurationManager.AppSettings["PathToBooks"] + @"\" + folder + @"\!Preview" + @"\" + id;
            string dirName = dir + @"\" + folder + @"\" + id;
            if (Directory.Exists(previewDirName))
            {
                string[] files = Directory.GetFiles(previewDirName);
                var image = files
                    .Select(fn => new BookDTO { FileName = Path.GetFileName(fn), FolderName = Path.GetFileName(previewDirName), FileDescription = new JpegMetadataAdapter(dirName + @"\" + Path.GetFileName(fn)).Metadata.Title, FileNameWithoutExtension = Convert.ToInt32(Path.GetFileNameWithoutExtension(fn)), })
                    .OrderBy(g => g.FileNameWithoutExtension)
                    .ToList();
                ViewBag.Response = image;
            }
            return View();
        }

        //Синхронизация
        [HttpGet]
        public ActionResult Synchronization()
        {
            //Очищаем таблицу Man-Photos (type = book) от старых значений
            IEnumerable<Man_Photo> manPhotos = db.Man_Photos.Where(s => s.GalleryType == "book");
            foreach (var os in manPhotos)
            {
                db.Man_Photos.Remove(os);
            }
            db.SaveChanges();


            var wrapFolders = System.IO.Directory.EnumerateDirectories(dir);
            foreach (var wf in wrapFolders)
            {
                var FullWrapFolderName = wf.ToString();
                var WrapFolderName = FullWrapFolderName.Replace(dir + "\\", "");
                var previewDir = FullWrapFolderName + @"\!Preview";

                if (!Directory.Exists(previewDir))
                {
                    Directory.CreateDirectory(previewDir);
                };

                var folders = System.IO.Directory.EnumerateDirectories(FullWrapFolderName);
                foreach (var b in folders)
                {
                    var FullFolderName = b.ToString();
                    var FolderName = FullFolderName.Replace(FullWrapFolderName + "\\", "");
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
                            if (!previewFileInf.Exists && (ext == ".jpg"))
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
                                        foreach(var m in photoMen)
                                        {
                                            Man_Photo mp = new Man_Photo();
                                            mp.UserId = m.Id;
                                            mp.GalleryType = "book";
                                            mp.FolderName = FolderName;
                                            mp.OrgFolderName = WrapFolderName;
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
            }
            return View();
        }


    }
}