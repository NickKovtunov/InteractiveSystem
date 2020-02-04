using interactiveSystem.Models;
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
    public class LeadershipController : Controller
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
        public ActionResult Index()
        {
            IEnumerable<Leadership> leaderships = db.Leaderships;
            ViewBag.Leaderships = leaderships;
            return View();
        }

        //Добавление
        [Authorize(Roles = "GD-URENGOY\\editors-IS")]
        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "GD-URENGOY\\editors-IS")]
        [ValidateInput(false)]
        public ActionResult Add(Leadership leadership, HttpPostedFileBase upload)
        {
            leadership.Description = leadership.Description;
            if (upload != null)
            {
                byte[] imageData = null;
                using (var binaryReader = new BinaryReader(upload.InputStream))
                {
                    imageData = binaryReader.ReadBytes(upload.ContentLength);
                }
                leadership.Photo = imageData;
            }
            db.Leaderships.Add(leadership);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        
        //Редактирование материала
        [HttpGet]
        [Authorize(Roles = "GD-URENGOY\\editors-IS")]
        public ActionResult Edit(int id)
        {
            Leadership EditLeadership = db.Leaderships.Find(id);
            ViewBag.EditLeadership = EditLeadership;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "GD-URENGOY\\editors-IS")]
        [ValidateInput(false)]
        public ActionResult Edit(int id, Leadership leadership, HttpPostedFileBase uploadImage)
        {
            if (id == leadership.Id)
            {
                if (leadership.Photo == null)
                {
                    if (uploadImage != null)
                    {
                        byte[] imageData = null;
                        using (var binaryReader = new BinaryReader(uploadImage.InputStream))
                        {
                            imageData = binaryReader.ReadBytes(uploadImage.ContentLength);
                        }
                        leadership.Photo = imageData;
                    }
                    else
                    {
                        leadership.Photo = null;
                    }
                }

                db.Entry(leadership).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        //Удаление
        [HttpPost]
        [Authorize(Roles = "GD-URENGOY\\editors-IS")]
        public ActionResult Delete(int id)
        {
            Leadership leadership = db.Leaderships.Find(id);
            if (leadership != null)
            {
                db.Leaderships.Remove(leadership);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        //Отображение списка руководителей с фотографиями для пользователей
        public ActionResult UserLeaderships()
        {
            IEnumerable<Leadership> leaderships = db.Leaderships.OrderBy(s => s.Order);
            ViewBag.Leaderships = leaderships;
            return View();
        }

        //Отображение списка руководителей с заглушками для пользователей
        public ActionResult UserLeaderships2()
        {
            IEnumerable<Leadership> leaderships = db.Leaderships.OrderBy(s => s.Order);
            ViewBag.Leaderships = leaderships;
            return View();
        }

        //Отображение списка руководителей без фотографий для пользователей
        public ActionResult UserLeaderships3()
        {
            IEnumerable<Leadership> leaderships = db.Leaderships.OrderBy(s => s.Order);
            ViewBag.Leaderships = leaderships;
            return View();
        }

        //Отображение описания руководителя для пользователей
        public ActionResult UserLeadershipDescription(int id)
        {
            Leadership Leadership = db.Leaderships.Find(id);
            ViewBag.Leadership = Leadership;
            return View();
        }
    }
}