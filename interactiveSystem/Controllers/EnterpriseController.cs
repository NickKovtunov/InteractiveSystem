using interactiveSystem.Models;
using KBCsv;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace interactiveSystem.Controllers
{
    public class EnterpriseController : Controller
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
            IEnumerable<Enterprise> enterprises = db.Enterprises.OrderBy(s=>s.Title);
            ViewBag.Enterprises = enterprises;
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
        public ActionResult Add(Enterprise enterprise, HttpPostedFileBase upload)
        {
            if (upload != null)
            {
                byte[] imageData = null;
                using (var binaryReader = new BinaryReader(upload.InputStream))
                {
                    imageData = binaryReader.ReadBytes(upload.ContentLength);
                }
                enterprise.Photo = imageData;
            }
            db.Enterprises.Add(enterprise);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //Редактирование материала
        [HttpGet]
        [Authorize(Roles = "GD-URENGOY\\editors-IS")]
        public ActionResult Edit(int id)
        {
            Enterprise EditEnterprise = db.Enterprises.Find(id);
            ViewBag.EditEnterprise = EditEnterprise;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "GD-URENGOY\\editors-IS")]
        [ValidateInput(false)]
        public ActionResult Edit(int id, Enterprise enterprise, HttpPostedFileBase uploadImage)
        {
            if (id == enterprise.Id)
            {
                if (enterprise.Photo == null)
                {
                    if (uploadImage != null)
                    {
                        byte[] imageData = null;
                        using (var binaryReader = new BinaryReader(uploadImage.InputStream))
                        {
                            imageData = binaryReader.ReadBytes(uploadImage.ContentLength);
                        }
                        enterprise.Photo = imageData;
                    }
                    else
                    {
                        enterprise.Photo = null;
                    }
                }

                db.Entry(enterprise).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        //Удаление
        [HttpPost]
        [Authorize(Roles = "GD-URENGOY\\editors-IS")]
        public ActionResult Delete(int id)
        {
            Enterprise enterprise = db.Enterprises.Find(id);
            if (enterprise != null)
            {
                db.Enterprises.Remove(enterprise);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        //Отображение предприятий для пользователей
        public ActionResult UserEnterprises()
        {
            IEnumerable<Enterprise> enterprises = db.Enterprises.Where(a => a.Type == 0).OrderBy(b => b.Order).ThenBy(b => b.Title);
            ViewBag.Enterprises = enterprises;
            return View();
        }

        //Отображение предприятий для пользователей
        public ActionResult UserArchive()
        {
            IEnumerable<Enterprise> enterprises = db.Enterprises.Where(a => a.Type == 1).OrderBy(b => b.Order).ThenBy(b => b.Title);
            ViewBag.Enterprises = enterprises;
            return View();
        }

        //Отображение описания предприятия для пользователей
        public ActionResult UserEnterpriseDescription(int id)
        {
            Enterprise FullEnterprise = db.Enterprises.Find(id);
            ViewBag.FullEnterprise = FullEnterprise;

            string str = FullEnterprise.Description;
            if (str != null)
            {
                char ch = '\n';
                int indexOfChar = str.IndexOf(ch);
                int strLength = str.Length;

                if ((indexOfChar < strLength) && (indexOfChar >= 0))
                {
                    ViewBag.FullEnterpriseShortDescription = FullEnterprise.Description.Substring(0, indexOfChar - 1);
                }
                else
                {
                    ViewBag.FullEnterpriseShortDescription = "";
                }
            }else
            {
                ViewBag.FullEnterpriseShortDescription = "";
            }

            string previewDir = ConfigurationManager.AppSettings["PathToBooks"] + @"\" + id + @"\!Preview";
            if (Directory.Exists(previewDir))
            {
                var folders = System.IO.Directory.EnumerateDirectories(previewDir);
                var galleries = folders
                    .Select(fn => Books.Create(fn))
                    .Where(g => g != null)
                    .OrderBy(g => g.OccurredOn);
                ViewBag.Books = galleries;
            }

            var EnterprisePeople = from e in db.Enterprises.Where(e => e.Id == id)
                                   join me in db.Man_Enterprises on e.Id equals me.EnterpriseId
                                   join m in db.Men on me.ManId equals m.Id
                                   select new ManEnterpriseDTO
                                   {
                                       Id = e.Id,
                                       ManId = m.Id,
                                       Man = m.Name,
                                       //Enterprise = e.Title,
                                       //Description = me.Description,
                                       //IsLeader = me.IsLeader,
                                       //StartDate = me.StartDate,
                                       //EndDate = me.EndDate
                                   };
            ViewBag.EnterprisePeople = EnterprisePeople.Distinct().OrderBy(s => s.Man);
            ViewBag.PeopleQuantity = EnterprisePeople.Count();
            return View();
        }
        
        //запрос для заполнения таблицы из csv-файла
        [HttpGet]
        [Authorize(Roles = "GD-URENGOY\\editors-IS")]
        public ActionResult SyncCSV()
        {
            List<Enterprise> EnterprisesDisplay = new List<Enterprise>();
            using (var streamReader = new StreamReader(@"D:\Projects\InteractiveSystem\interactiveSystem\Content\Enterprise.csv"))
            using (var reader = new CsvReader(streamReader))
            {
                reader.ReadHeaderRecord();
                while (reader.HasMoreRecords)
                {
                    var dataRecord = reader.ReadDataRecord();
                    string row = dataRecord.ToString();
                    string[] rows = row.Split(new char[] { ';' });

                    Enterprise EnterpriseDisplay = new Enterprise();
                    int x1 = 0;
                    x1 = rows[1].Length - 1;
                    rows[1] = rows[1].Substring(0, x1);
                    EnterpriseDisplay.Title = rows[1];
                    EnterpriseDisplay.IdOrg = int.Parse(rows[0]);
                    EnterprisesDisplay.Add(EnterpriseDisplay);
                }
            }

            foreach(var e in EnterprisesDisplay)
            {
                IEnumerable<Enterprise> OldEnterprises = db.Enterprises.Where(st => st.IdOrg == e.IdOrg);
                if (OldEnterprises.Count() == 0)
                {
                    db.Enterprises.Add(e);
                    db.SaveChanges();
                }
            }

            ViewBag.List = EnterprisesDisplay;

            return View();
        }
    }
}