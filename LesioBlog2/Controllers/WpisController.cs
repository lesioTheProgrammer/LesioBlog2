using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LeisoBlog2_Repo.Abstract;
using LesioBlog2_Repo.Models;
using LesioBlog2_Repo.Models.Context;


namespace LesioBlog2.Controllers
{
    public class WpisController : Controller
    {
        private readonly IWpisRepo  _wpis;

        public WpisController(IWpisRepo wpisrepo)
        {
            this._wpis = wpisrepo;
        }


        // GET: Wpis
        public ActionResult Index()
        {
            var wpis = _wpis.GetWpis();
            return View(wpis.ToList());
        }

        // GET: Wpis/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Wpis wpis = _wpis.GetWpisById(id);
            if (wpis == null)
            {
                return HttpNotFound();
            }
            return View(wpis);
        }

        // GET: Wpis/Create
        public ActionResult Create()
        {
          //  ViewBag.UserID = new SelectList(db.Users, "UserID", "NickName");
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
                _wpis.Add(wpis);
                _wpis.SaveChanges();
                return RedirectToAction("Index");
            }

           // ViewBag.UserID = new SelectList(db.Users, "UserID", "NickName", wpis.UserID);
            return View(wpis);
        }

        // GET: Wpis/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Wpis wpis = _wpis.GetWpisById(id);
            if (wpis == null)
            {
                return HttpNotFound();
            }
         //   ViewBag.UserID = new SelectList(db.Users, "UserID", "NickName", wpis.UserID);
            return View(wpis);
        }

        // POST: Wpis/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "WpisID,UserID,Content,Plusy,AddingDate")] Wpis wpis)
        {
            wpis.AddingDate = _wpis.GetWpisWithAddDate(wpis);



            if (ModelState.IsValid)
            {
                _wpis.Update(wpis);
                _wpis.SaveChanges();
                return RedirectToAction("Index");
            }
           // ViewBag.UserID = new SelectList(db.Users, "UserID", "NickName", wpis.UserID);
            return View(wpis);
        }

        // GET: Wpis/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Wpis wpis = _wpis.GetWpisById(id);
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
            Wpis wpis = _wpis.GetWpisById(id);
            _wpis.Delete(wpis);
            _wpis.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _wpis.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
