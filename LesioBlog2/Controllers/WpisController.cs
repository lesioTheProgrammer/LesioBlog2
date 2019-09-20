using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LesioBlog2_Repo.Models;

namespace LesioBlog2.Controllers
{
    public class WpisController : Controller
    {
        private BlogContext db = new BlogContext();

        // GET: Wpis
        public ActionResult Index()
        {
            var wpis = db.Wpis.Include(w => w.User);
            return View(wpis.ToList());
        }

        // GET: Wpis/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Wpis wpis = db.Wpis.Find(id);
            if (wpis == null)
            {
                return HttpNotFound();
            }
            return View(wpis);
        }

        // GET: Wpis/Create
        public ActionResult Create()
        {
            ViewBag.UserID = new SelectList(db.Users, "UserID", "NickName");
            return View();
        }

        // POST: Wpis/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "WpisID,UserID,Content,AddingDate,Plusy")] Wpis wpis)
        {
            if (ModelState.IsValid)
            {
                db.Wpis.Add(wpis);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserID = new SelectList(db.Users, "UserID", "NickName", wpis.UserID);
            return View(wpis);
        }

        // GET: Wpis/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Wpis wpis = db.Wpis.Find(id);
            if (wpis == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserID = new SelectList(db.Users, "UserID", "NickName", wpis.UserID);
            return View(wpis);
        }

        // POST: Wpis/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "WpisID,UserID,Content,AddingDate,Plusy")] Wpis wpis)
        {
            if (ModelState.IsValid)
            {
                db.Entry(wpis).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserID = new SelectList(db.Users, "UserID", "NickName", wpis.UserID);
            return View(wpis);
        }

        // GET: Wpis/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Wpis wpis = db.Wpis.Find(id);
            if (wpis == null)
            {
                return HttpNotFound();
            }
            return View(wpis);
        }

        // POST: Wpis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Wpis wpis = db.Wpis.Find(id);
            db.Wpis.Remove(wpis);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
