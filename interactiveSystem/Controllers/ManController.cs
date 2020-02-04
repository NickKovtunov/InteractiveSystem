using interactiveSystem.Models;
using KBCsv;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace interactiveSystem.Controllers
{
    public class ManController : Controller
    {
        private Guid? GetGuidByIdentity(string identity)
        {
            using (var context = new PrincipalContext(ContextType.Domain, "gd-urengoy"))
            {
                UserPrincipal principal = UserPrincipal.FindByIdentity(context, identity);
                if (principal != null)
                {
                    return principal.Guid; //principal.DisplayName;
                }
            }
            return null;
        }

        ISContext db = new ISContext();
        [Authorize(Roles = "GD-URENGOY\\editors-IS")]
        public ActionResult Index()
        {
            IEnumerable<Man> men = db.Men.OrderBy(s=>s.Name);
            ViewBag.Men = men;
            return View();
        }

        //Добавление
        [HttpGet]
        [Authorize(Roles = "GD-URENGOY\\editors-IS")]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "GD-URENGOY\\editors-IS")]
        [ValidateInput(false)]
        public ActionResult Add(Man man, HttpPostedFileBase upload)
        {
            if (upload != null)
            {
                byte[] imageData = null;
                using (var binaryReader = new BinaryReader(upload.InputStream))
                {
                    imageData = binaryReader.ReadBytes(upload.ContentLength);
                }
                man.Photo = imageData;
            }
            db.Men.Add(man);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        
        //Редактирование материала
        [HttpGet]
        [Authorize(Roles = "GD-URENGOY\\editors-IS")]
        public ActionResult Edit(int id)
        {
            Man EditMan = db.Men.Find(id);
            ViewBag.EditMan = EditMan;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "GD-URENGOY\\editors-IS")]
        [ValidateInput(false)]
        public ActionResult Edit(int id, Man man, HttpPostedFileBase uploadImage)
        {
            if (id == man.Id)
            {
                if (man.Photo == null)
                {
                    if (uploadImage != null)
                    {
                        byte[] imageData = null;
                        using (var binaryReader = new BinaryReader(uploadImage.InputStream))
                        {
                            imageData = binaryReader.ReadBytes(uploadImage.ContentLength);
                        }
                        man.Photo = imageData;
                    }
                    else
                    {
                        man.Photo = null;
                    }
                }

                db.Entry(man).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        //Удаление
        [HttpPost]
        [Authorize(Roles = "GD-URENGOY\\editors-IS")]
        public ActionResult Delete(int id)
        {
            Man man = db.Men.Find(id);
            if (man != null)
            {
                db.Men.Remove(man);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        //Отображение описания человека для пользователей
        public ActionResult UserManDescription(int id)
        {
            Man FullMan = db.Men.Find(id);
            ViewBag.FullMan = FullMan;

            //var ManRewards = from r in db.Rewards.Where(r => r.Id == id)
            //                   join mr in db.Man_Rewards on r.Id equals mr.RewardId
            //                   join m in db.Men on mr.ManId equals m.Id
            //                   select new ManRewardDTO
            //                   {
            //                       Id = r.Id,
            //                       Reward = r.Title,
            //                       ManId = m.Id,
            //                       Man = m.Name,
            //                       Description = mr.Description,
            //                       Date = mr.Date
            //                   };
            var ManRewards = from m in db.Men.Where(m => m.Id == id)
                             join mr in db.Man_Rewards on m.Id equals mr.ManId
                             join r in db.Rewards on mr.RewardId equals r.Id
                             select new ManRewardDTO
                             {
                                 RewardId = r.Id,
                                 Reward = r.Title,
                                 RewardPhoto = r.Photo,
                                 ManId = m.Id,
                                 Man = m.Name,
                                 Description = mr.Description,
                                 Date = mr.Date
            };
            ViewBag.ManRewards = ManRewards;

            var ManEnterprises = from m in db.Men.Where(m => m.Id == id)
                             join me in db.Man_Enterprises on m.Id equals me.ManId
                             join e in db.Enterprises on me.EnterpriseId equals e.Id
                             select new ManEnterpriseDTO
                             {
                                 Id = e.Id,
                                 ManId = m.Id,
                                 Man = m.Name,
                                 Enterprise = e.Title,
                                 Description = me.Description,
                                 IsLeader = me.IsLeader,
                                 StartDate = me.StartDate,
                                 EndDate = me.EndDate
                             };
            ViewBag.ManEnterprises = ManEnterprises.OrderByDescending(s => s.StartDate);

            IEnumerable<Man_Photo> ManPhotos = db.Man_Photos.Where(s => s.UserId == id);
            ViewBag.ManPhotos = ManPhotos;

            return View();
        }

        //запрос для заполнения таблицы из csv-файла
        [HttpGet]
        [Authorize(Roles = "GD-URENGOY\\editors-IS")]
        public ActionResult SyncCSV()
        {
            List<Man> MenDisplay = new List<Man>();
            using (var streamReader = new StreamReader(@"D:\Projects\InteractiveSystem\interactiveSystem\Content\Man.csv"))
            using (var reader = new CsvReader(streamReader))
            {
                reader.ReadHeaderRecord();
                while (reader.HasMoreRecords)
                {
                    var dataRecord = reader.ReadDataRecord();
                    string row = dataRecord.ToString();
                    string[] rows = row.Split(new char[] { ';' });

                    Man ManDisplay = new Man();
                    ManDisplay.Name = rows[2];
                    ManDisplay.IdPersonal = int.Parse(rows[1]);
                    MenDisplay.Add(ManDisplay);
                }
            }

            foreach (var e in MenDisplay)
            {
                IEnumerable<Man> OldMen = db.Men.Where(st => st.IdPersonal == e.IdPersonal);
                if (OldMen.Count() == 0)
                {
                    db.Men.Add(e);
                    db.SaveChanges();
                }
            }

            ViewBag.List = MenDisplay;

            return View();
        }

        //запрос для заполнения таблицы из csv-файла
        [HttpGet]
        [Authorize(Roles = "GD-URENGOY\\editors-IS")]
        public ActionResult SyncBirthdayAndPositionCSV()
        {
            List<Man> MenDisplay = new List<Man>();
            //using (var streamReader = new StreamReader(@"D:\Projects\InteractiveSystem\interactiveSystem\Content\Man-birthday-position.csv"))
            using (var streamReader = new StreamReader(@"D:\Projects\InteractiveSystem\interactiveSystem\Content\Man-birthday-position2.csv"))
            using (var reader = new CsvReader(streamReader))
            {
                reader.ReadHeaderRecord();
                while (reader.HasMoreRecords)
                {
                    var dataRecord = reader.ReadDataRecord();
                    string row = dataRecord.ToString();
                    string[] rows = row.Split(new char[] { ';' });

                    Man ManDisplay = new Man();
                    if (rows[7] != "")
                    {
                        ManDisplay.Birthday = Convert.ToDateTime(rows[7]);
                    }
                    if (rows[1] != "")
                    {
                        ManDisplay.IdPersonal = int.Parse(rows[1]);
                    }
                    ManDisplay.Position = rows[23];

                    MenDisplay.Add(ManDisplay);
                }
            }

            foreach (var e in MenDisplay)
            {
                IEnumerable<Man> EditMen = db.Men.Where(st => st.IdPersonal == e.IdPersonal);
                if (EditMen.Count() != 0)
                {
                    Man EditMan = new Man();
                    EditMan = EditMen.First();
                    EditMan.Position = e.Position;
                    EditMan.Birthday = e.Birthday;
                    db.Entry(EditMan).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            ViewBag.List = MenDisplay;

            return View();
        }

        //запрос для синхронизации фотографий пользователей
        [HttpGet]
        [Authorize(Roles = "GD-URENGOY\\editors-IS")]
        public ActionResult SyncMenPhotos()
        {
            string dirName = @"D:\Photos";
            if (Directory.Exists(dirName))
            {
                string[] files = Directory.GetFiles(dirName);
                foreach (var f in files)
                {
                    int? fileName = Convert.ToInt32(Path.GetFileNameWithoutExtension(f));
                    Byte[] fileByte = System.IO.File.ReadAllBytes(f);

                    IEnumerable<Man> Men = db.Men.Where(st => st.IdPersonal == fileName);
                    foreach (var m in Men)
                    {
                            m.Photo = fileByte;
                            db.Entry(m).State = EntityState.Modified;
                    }
                }
                db.SaveChanges();
            }
            
            return View();
        }
    }
}