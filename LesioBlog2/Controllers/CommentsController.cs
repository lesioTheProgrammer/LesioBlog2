using LesioBlog2_Repo.Abstract;
using LesioBlog2_Repo.Models;
using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace LesioBlog2.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ICommentRepo _comm;
        private readonly IUserRepo _user;
        private readonly ITagRepo _tag;

        public CommentsController(ICommentRepo commm, ITagRepo tagrepo, IUserRepo users)
        {
            this._comm = commm;
            this._user = users;
            this._tag = tagrepo;
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
          //  comment.WpisID = 8;
            //wpisID is not passed?????
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


                MatchCollection matches = Regex.Matches(comment.Content, @"\B(\#[a-zA-Z0-9-,_]+\b)");
                //wpis ID first important
                foreach (var tagName in matches)
                {
                    var tag = _tag.GetTagByName(tagName.ToString());
                    if (tag == null)
                    {
                        tag = new Tag();
                        tag.TagName = tagName.ToString();
                        //id radnom
                        _tag.Add(tag);
                        _tag.SaveChanges();
                    }

                    var commTag = new CommentTag()
                    {
                        TagID = tag.TagID,
                        CommentID= comment.CommentID
                    };
                    _comm.Add(commTag);
                    _comm.SaveChanges();
                }

                return RedirectToAction("Index", "Wpis");
            }
            return PartialView(comment);
        }

        // GET: Comments/Edit/5
        [AuthorizeUserAttribute]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var IdOfCreator = _comm.GetIdOfCommentCreator(id);
            int? currentlyLoggedUserId = _user.GetIDOfCurrentlyLoggedUser();
            if (currentlyLoggedUserId == null)
            {
                return RedirectToAction("LogIn", "User");
            }

            if (IdOfCreator == currentlyLoggedUserId)
            {
                Comment comment = _comm.GetCommentById(id);
                if (comment == null)
                {
                    return HttpNotFound();
                }
                return View(comment);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "You can't edit someone else comment gierarka hir \n FOR REAL");
            }
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,UserID,WpisID,Content,AddingDate,Plusy")] Comment comment)
        {
            comment.Plusy = 0;
            comment.EditingDate = DateTime.Now;
            comment.AddingDate = _comm.GetCommentWithAddingDate(comment);

            if (ModelState.IsValid)
            {
                _comm.UpdateContentAndPlusyAndEditDate(comment);
                _comm.SaveChanges();
                return RedirectToAction("Index", "Wpis");

            }
            return View(comment);
        }

        // GET: Comments/Delete/5
        [AuthorizeUserAttribute]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            bool isUserLogged = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            var IdOfCreator = _comm.GetIdOfCommentCreator(id);
            int? currentlyLoggedUserId = _user.GetIDOfCurrentlyLoggedUser();
            if (currentlyLoggedUserId == null)
            {
                return RedirectToAction("LogIn", "User");
            }
            if (isUserLogged && IdOfCreator == currentlyLoggedUserId)
            {
                Comment comment = _comm.GetCommentById(id);
                if (comment == null)
                {
                    return HttpNotFound();
                }
                return View(comment);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "You can't DELETE someone else comment gierarka hir \n FOR REAL");
            }
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeUserAttribute]
        public ActionResult DeleteConfirmed(int id)
        {
            Comment comment = _comm.GetCommentById(id);
            _comm.Delete(comment);
            _comm.SaveChanges();
            return RedirectToAction("Index", "Wpis");
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
