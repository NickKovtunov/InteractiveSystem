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
    public class ManRewardController : Controller
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
            //IEnumerable<Man_Reward> mrs = db.Man_Rewards.Where(m => m.ManId == id);
            //var mrs = db.Man_Rewards.Where(m => m.ManId == id).Join(db.Rewards,
            //    mr => mr.RewardId,
            //    r => r.Id,
            //    (mr, r) => new ManRewardDTO
            //    {
            //        Id = mr.Id,
            //        ManId = mr.ManId,
            //        Reward = r.Title,
            //        Description = mr.Description,
            //        Date = mr.Date
            //    }).ToList();
            var mrs = from mr in db.Man_Rewards.Where(m => m.ManId == id)
                      join r in db.Rewards on mr.RewardId equals r.Id
                      join m in db.Men on mr.ManId equals m.Id
                      select new ManRewardDTO
                      {
                          Id = mr.Id,
                          ManId = mr.ManId,
                          Man = m.Name,
                          Reward = r.Title,
                          Description = mr.Description,
                          Date = mr.Date
                      };
            ViewBag.Man_Rewards = mrs.OrderBy(s => s.Reward);
            ViewBag.AddMrId = id;
            return View();
        }

        //Добавление
        [HttpGet]
        public ActionResult Add(int id)
        {
            ViewBag.AddMrId = id;
            IEnumerable<Reward> rewards = db.Rewards.OrderBy(s => s.Title);
            ViewBag.Rewards = rewards;
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(Man_Reward mr)
        {
            db.Man_Rewards.Add(mr);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = mr.ManId });
        }

        //Редактирование материала
        [HttpGet]
        public ActionResult Edit(int id)
        {
            IEnumerable<Reward> rewards = db.Rewards;
            ViewBag.Rewards = rewards;
            Man_Reward EditMr = db.Man_Rewards.Find(id);
            ViewBag.EditMr = EditMr;
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(int id, Man_Reward mr)
        {
            if (id == mr.Id)
            {
                db.Entry(mr).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index", new { id = mr.ManId });
        }


        [HttpPost]
        public ActionResult Delete(int id)
        {
            Man_Reward mr = db.Man_Rewards.Find(id);
            if (mr != null)
            {
                db.Man_Rewards.Remove(mr);
                db.SaveChanges();
            }
            return RedirectToAction("Index", new { id = mr.ManId });
        }

        //запрос для заполнения таблицы из csv-файла
        [HttpGet]
        [Authorize(Roles = "GD-URENGOY\\editors-IS")]
        public ActionResult SyncCSV()
        {
            List<Man_Reward> MRsDisplayStart = new List<Man_Reward>();
            List<Man_Reward> MRsDisplayEnd = new List<Man_Reward>();
            using (var streamReader = new StreamReader(@"D:\Projects\InteractiveSystem\interactiveSystem\Content\ManReward.csv"))
            using (var reader = new CsvReader(streamReader))
            {
                reader.ReadHeaderRecord();
                while (reader.HasMoreRecords)
                {
                    var dataRecord = reader.ReadDataRecord();
                    string row = dataRecord.ToString();
                    string[] rows = row.Split(new char[] { ';' });
                    if ((rows[0] != "") && (rows[2] != ""))
                    {
                        Man_Reward MRDisplayStart = new Man_Reward();
                        MRDisplayStart.ManId = int.Parse(rows[0]);
                        MRDisplayStart.RewardId = int.Parse(rows[2]);
                        MRDisplayStart.Description = rows[8];
                        if (rows[5] != "")
                        {
                            MRDisplayStart.Date = Convert.ToDateTime(rows[5]);
                        }
                        MRsDisplayStart.Add(MRDisplayStart);
                    }
                }
            }

            foreach (var e in MRsDisplayStart)
            {
                IEnumerable<Man> MenDisplayEnd = db.Men.Where(st => st.IdPersonal == e.ManId);
                IEnumerable<Reward> RewardsDisplayEnd = db.Rewards.Where(st => st.IdRew == e.RewardId);
                if ((MenDisplayEnd.Count() != 0) && (RewardsDisplayEnd.Count() != 0))
                {
                    Man_Reward MRDisplayEnd = new Man_Reward();
                    MRDisplayEnd.ManId = MenDisplayEnd.First().Id;
                    MRDisplayEnd.RewardId = RewardsDisplayEnd.First().Id;
                    MRDisplayEnd.Description = e.Description;
                    MRDisplayEnd.Date = e.Date;
                    MRsDisplayEnd.Add(MRDisplayEnd);
                    db.Man_Rewards.Add(MRDisplayEnd);
                    db.SaveChanges();
                }
            }

            ViewBag.List = MRsDisplayEnd;

            return View();
        }
    }
}