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
    public class RewardController : Controller
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
            IEnumerable<Reward> rewards = db.Rewards.OrderBy(r => r.Type).ThenBy(r => r.Order).ThenBy(r => r.Title);
            ViewBag.Rewards = rewards;
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
        public ActionResult Add(Reward reward, HttpPostedFileBase upload)
        {
            if (upload != null)
            {
                byte[] imageData = null;
                using (var binaryReader = new BinaryReader(upload.InputStream))
                {
                    imageData = binaryReader.ReadBytes(upload.ContentLength);
                }
                reward.Photo = imageData;
            }
            db.Rewards.Add(reward);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //Редактирование материала
        [HttpGet]
        [Authorize(Roles = "GD-URENGOY\\editors-IS")]
        public ActionResult Edit(int id)
        {
            Reward EditReward = db.Rewards.Find(id);
            ViewBag.EditReward = EditReward;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "GD-URENGOY\\editors-IS")]
        [ValidateInput(false)]
        public ActionResult Edit(int id, Reward reward, HttpPostedFileBase uploadImage)
        {
            if (id == reward.Id)
            {
                if (reward.Photo == null)
                {
                    if (uploadImage != null)
                    {
                        byte[] imageData = null;
                        using (var binaryReader = new BinaryReader(uploadImage.InputStream))
                        {
                            imageData = binaryReader.ReadBytes(uploadImage.ContentLength);
                        }
                        reward.Photo = imageData;
                    }
                    else
                    {
                        reward.Photo = null;
                    }
                }

                db.Entry(reward).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        //Удаление
        [HttpPost]
        [Authorize(Roles = "GD-URENGOY\\editors-IS")]
        public ActionResult Delete(int id)
        {
            Reward reward = db.Rewards.Find(id);
            if (reward != null)
            {
                db.Rewards.Remove(reward);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        //Отображение наград для пользователей
        public ActionResult UserRewards()
        {
            IEnumerable<Reward> rewards = db.Rewards.OrderBy(r => r.Type).ThenBy(r => r.Order).ThenBy(r => r.Title);
            ViewBag.Rewards = rewards;
            return View();
        }

        //Отображение описания награды для пользователей
        public ActionResult UserRewardDescription(int id)
        {
            Reward FullReward = db.Rewards.Find(id);
            ViewBag.FullReward = FullReward;

            string str = FullReward.Description;
            if (str != null)
            {
                char ch = '\n';
                int indexOfChar = str.IndexOf(ch);
                int strLength = str.Length;

                if ((indexOfChar < strLength) && (indexOfChar >= 0))
                {
                    ViewBag.FullRewardShortDescription = FullReward.Description.Substring(0, indexOfChar - 1);
                }
                else
                {
                    ViewBag.FullRewardShortDescription = "";
                }
            }
            else
            {
                ViewBag.FullRewardShortDescription = "";
            }

            var RewardPeople = from r in db.Rewards.Where(r => r.Id == id)
                             join mr in db.Man_Rewards on r.Id equals mr.RewardId
                             join m in db.Men on mr.ManId equals m.Id
                             select new ManRewardDTO
                             {
                                 Id = r.Id,
                                 Reward = r.Title,
                                 ManId = m.Id,
                                 Man = m.Name,
                                 Description = mr.Description,
                                 Date = mr.Date
                             };
            ViewBag.RewardPeople = RewardPeople.OrderBy(s => s.Man);
            ViewBag.PeopleQuantity = RewardPeople.Count();
            return View();
        }

        //запрос для заполнения таблицы из csv-файла
        [HttpGet]
        [Authorize(Roles = "GD-URENGOY\\editors-IS")]
        public ActionResult SyncCSV()
        {
            List<Reward> RewardsDisplay = new List<Reward>();
            using (var streamReader = new StreamReader(@"D:\Projects\InteractiveSystem\interactiveSystem\Content\Reward.csv"))
            using (var reader = new CsvReader(streamReader))
            {
                reader.ReadHeaderRecord();
                while (reader.HasMoreRecords)
                {
                    var dataRecord = reader.ReadDataRecord();
                    string row = dataRecord.ToString();
                    string[] rows = row.Split(new char[] { ';' });

                    Reward RewardDisplay = new Reward();
                    RewardDisplay.Title = rows[1];
                    RewardDisplay.IdRew = int.Parse(rows[0]);
                    RewardsDisplay.Add(RewardDisplay);
                }
            }

            foreach (var e in RewardsDisplay)
            {
                IEnumerable<Reward> OldRewards = db.Rewards.Where(st => st.IdRew == e.IdRew);
                if (OldRewards.Count() == 0)
                {
                    db.Rewards.Add(e);
                    db.SaveChanges();
                }
            }

            ViewBag.List = RewardsDisplay;

            return View();
        }
    }
}