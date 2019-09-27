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
    public class CommentsController : Controller
    {
        private readonly ICommentRepo _comm;
        private readonly IWpisRepo _wpis;

        public CommentsController(ICommentRepo commm, IWpisRepo wpis)
        {
            this._comm = commm;
            this._wpis = wpis;
        }

        // GET: Comments
        public ActionResult Index(string userNickName)
        {
            var comments = _comm.GetComment();
            if (string.IsNullOrEmpty(userNickName))
            {
              return View(comments.ToList());
            }
            else
            {
                comments = _comm.GetCommentByUserNickName(userNickName).AsQueryable();
            }
            return View(comments);

        }

        // GET: Comments/Details/5
        public ActionResult Details(int? id)
        {

            var comment = _comm.GetCommentById(id);

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // GET: Comments/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,UserID,WpisID,Content,AddingDate,Plusy")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            return View(comment);
        }

        // GET: Comments/Edit/5
        public ActionResult Edit(int? id)
        {
            var comment = _comm.GetCommentById(id);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,UserID,WpisID,Content,AddingDate,Plusy")] Comment comment)
        {

            if (ModelState.IsValid)
            {
                _comm.Update(comment);
                _comm.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(comment);
        }

        // GET: Comments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = _comm.GetCommentById(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comment comment = _comm.GetCommentById(id);
            _comm.Delete(comment);
            _comm.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _comm.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
