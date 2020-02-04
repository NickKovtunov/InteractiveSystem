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
    [Authorize(Roles = "GD-URENGOY\\editors-IS")]
    public class ManEnterpriseController : Controller
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
        public ActionResult Index(int id)
        {
            //IEnumerable<Man_Enterprise> mes = db.Man_Enterprises.Where(m => m.ManId == id);
            //var mes = db.Man_Enterprises.Where(m => m.ManId == id).Join(db.Enterprises,
            //    me => me.EnterpriseId,
            //    e => e.Id,
            //    (me, e) => new ManEnterpriseDTO
            //    {
            //        Id = me.Id,
            //        ManId = me.ManId,
            //        Man = me.ManId,
            //        Enterprise = e.Title,
            //        Description = me.Description,
            //        IsLeader = me.IsLeader,
            //        StartDate = me.StartDate,
            //        EndDate = me.EndDate
            //    }).ToList();
            var mes = from me in db.Man_Enterprises.Where(m => m.ManId == id)
                      join e in db.Enterprises on me.EnterpriseId equals e.Id
                      join m in db.Men on me.ManId equals m.Id
                      select new ManEnterpriseDTO
                        {
                            Id = me.Id,
                            ManId = me.ManId,
                            Man = m.Name,
                            Enterprise = e.Title,
                            Description = me.Description,
                            IsLeader = me.IsLeader,
                            StartDate = me.StartDate,
                            EndDate = me.EndDate
                        };
            ViewBag.Man_Enterprises = mes.OrderBy(s=>s.Enterprise);
            ViewBag.AddMeId = id;
            return View();
        }

        //Добавление
        [HttpGet]
        public ActionResult Add(int id)
        {
            ViewBag.AddMeId = id;
            IEnumerable<Enterprise> enterprises = db.Enterprises.OrderBy(s => s.Title);
            ViewBag.Enterprises = enterprises;
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(Man_Enterprise me)
        {
            db.Man_Enterprises.Add(me);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = me.ManId });
        }

        //Редактирование материала
        [HttpGet]
        public ActionResult Edit(int id)
        {
            IEnumerable<Enterprise> enterprises = db.Enterprises.OrderBy(s => s.Title);
            ViewBag.Enterprises = enterprises;
            Man_Enterprise EditMe = db.Man_Enterprises.Find(id);
            ViewBag.EditMe = EditMe;
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(int id, Man_Enterprise me)
        {
            if (id == me.Id)
            {
                db.Entry(me).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index", new { id = me.ManId });
        }


        [HttpPost]
        public ActionResult Delete(int id)
        {
            Man_Enterprise me = db.Man_Enterprises.Find(id);
            if (me != null)
            {
                db.Man_Enterprises.Remove(me);
                db.SaveChanges();
            }
            return RedirectToAction("Index", new { id = me.ManId });
        }

        //запрос для заполнения таблицы из csv-файла
        [HttpGet]
        [Authorize(Roles = "GD-URENGOY\\editors-IS")]
        public ActionResult SyncCSV()
        {
            List<Man_Enterprise> MEsDisplayStart = new List<Man_Enterprise>();
            List<Man_Enterprise> MEsDisplayEnd = new List<Man_Enterprise>();
            using (var streamReader = new StreamReader(@"D:\Projects\InteractiveSystem\interactiveSystem\Content\ManEnterprise.csv"))
            using (var reader = new CsvReader(streamReader))
            {
                reader.ReadHeaderRecord();
                while (reader.HasMoreRecords)
                {
                    var dataRecord = reader.ReadDataRecord();
                    string row = dataRecord.ToString();
                    string[] rows = row.Split(new char[] { ';' });
                    if((rows[0]!="")&& (rows[2] != ""))
                    {
                        Man_Enterprise MEDisplayStart = new Man_Enterprise();
                        MEDisplayStart.ManId = int.Parse(rows[0]);
                        MEDisplayStart.EnterpriseId = int.Parse(rows[2]);
                        if (rows[4] != "")
                        {
                            MEDisplayStart.StartDate = Convert.ToDateTime(rows[4]);
                        }

                        int x1 = 0;
                        x1 = rows[5].Length - 1;
                        rows[5] = rows[5].Substring(0, x1);

                        if ((rows[5] != "")&&(rows[5] != "01.янв.9999 12:00:00 AM"))
                        {
                            MEDisplayStart.EndDate = Convert.ToDateTime(rows[5]);
                        }
                        MEsDisplayStart.Add(MEDisplayStart);
                    }
                }
            }

            foreach (var e in MEsDisplayStart)
            {
                IEnumerable<Man> MenDisplayEnd = db.Men.Where(st => st.IdPersonal == e.ManId);
                IEnumerable<Enterprise> EnterpriseDisplayEnd = db.Enterprises.Where(st => st.IdOrg == e.EnterpriseId);
                if ((MenDisplayEnd.Count() != 0)&& (EnterpriseDisplayEnd.Count() != 0))
                {
                    Man_Enterprise MEDisplayEnd = new Man_Enterprise();
                    MEDisplayEnd.ManId = MenDisplayEnd.First().Id;
                    MEDisplayEnd.EnterpriseId = EnterpriseDisplayEnd.First().Id;
                    MEDisplayEnd.StartDate = e.StartDate;
                    MEDisplayEnd.EndDate = e.EndDate;
                    MEsDisplayEnd.Add(MEDisplayEnd);
                    db.Man_Enterprises.Add(MEDisplayEnd);
                    db.SaveChanges();
                }
            }

            ViewBag.List = MEsDisplayEnd;

            return View();
        }
    }
}