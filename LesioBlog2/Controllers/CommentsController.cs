using LeisoBlog2_Repo.Abstract;
using LesioBlog2_Repo.Models;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace LesioBlog2.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ICommentRepo _comm;
        private readonly IWpisRepo _wpis;
        private readonly IUserRepo _user;

        public CommentsController(ICommentRepo commm, IWpisRepo wpis, IUserRepo users)
        {
            this._comm = commm;
            this._wpis = wpis;
            this._user = users;
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
        public ActionResult Create([Bind(Include = "WpisID,Content,UserID,ID,AddingDate,Plusy,EditingDate")] Comment comment)
        {

            comment.Plusy = 0;
            comment.AddingDate = DateTime.Now;
            //load from users:
            var nullableUserID = _user.GetIDOfCurrentlyLoggedUser();
            if (nullableUserID == null)
            {
                return RedirectToAction("LogIn", "User");
            }
            else
            {
                //int? to int
                comment.UserID = nullableUserID.GetValueOrDefault();
            }
            comment.EditingDate = null;

            if (ModelState.IsValid)
            {
                _comm.Add(comment);
                _comm.SaveChanges();
                //string returnUrl = Request.UrlReferrer.AbsoluteUri;
               // return Redirect(returnUrl);

                // return new RedirectToRouteResult(new RouteValueDictionary(new { action = "Index", controller = "Wpis" }));
                return RedirectToAction("Index", "Wpis");
            }

            return PartialView(comment);
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
                //no need to redirect it 
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
