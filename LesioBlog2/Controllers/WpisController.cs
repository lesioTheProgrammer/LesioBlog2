using LeisoBlog2_Repo.Abstract;
using LesioBlog2_Repo.Models;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace LesioBlog2.Controllers
{
    public class WpisController : Controller
    {
        private readonly IWpisRepo _wpis;
        private readonly ITagRepo _tag;
        private readonly IUserRepo _user;

        public WpisController(IWpisRepo wpisrepo, ITagRepo tagrepo, IUserRepo user)
        {
            this._wpis = wpisrepo;
            this._tag = tagrepo;
            this._user = user;
        }


        // GET: Wpis


        public ActionResult Index(string userNickName)
        {
            var wpis = _wpis.GetWpis();
            if (string.IsNullOrEmpty(userNickName))
            {
                return View(wpis.ToList());
            }
            else
            {
                //get wpis by user id
                wpis = _wpis.GetWpisByUserNickName(userNickName).AsQueryable();

            }
            return View(wpis.ToList());

        }

        public ActionResult GoToParentWpis(int? id)
        {
            //comment ma wpisID
            //pokaz wpis po wpisID z komenta
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var wpis = _wpis.GetWpisById(id);
            return View(wpis);
        }




        // GET: Wpis/Details/5
        [AuthorizeUserAttribute]
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
            bool isUserLogged = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            var IdOfCreator = _wpis.GetIdOfWpisCreator(id);
            //id of logged user
            int? currentlyLoggedUserId = _user.GetIDOfCurrentlyLoggedUser();
            if (currentlyLoggedUserId == null)
            {
                return RedirectToAction("LogIn", "User");
            }
            //end
            if (isUserLogged && IdOfCreator == currentlyLoggedUserId)
            {
                Wpis wpis = _wpis.GetWpisById(id);
                if (wpis == null)
                {
                    return HttpNotFound();
                }
                return View(wpis);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "You can't edit someone else wpis gierarka hir \n FOR REAL");
            }

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
            bool isUserLogged = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            var IdOfCreator = _wpis.GetIdOfWpisCreator(id);
            //id of logged user
            int? currentlyLoggedUserId = _user.GetIDOfCurrentlyLoggedUser();
            if (currentlyLoggedUserId == null)
            {
                return RedirectToAction("LogIn", "User");
            }
            //end
            if (isUserLogged && IdOfCreator == currentlyLoggedUserId)
            {
                Wpis wpis = _wpis.GetWpisById(id);
                if (wpis == null)
                {
                    return HttpNotFound();
                }
                return View(wpis);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "You can't DELETE someone else wpis gierarka hir \n FOR REAL");
            }
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


    public class AuthorizeUserAttribute : AuthorizeAttribute
    {

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {

            bool isUserLogged = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;

            return isUserLogged;
        }
    }
}