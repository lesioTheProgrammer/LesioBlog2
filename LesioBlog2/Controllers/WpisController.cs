using LesioBlog2_Repo.Abstract;
using LesioBlog2_Repo.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

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
        public ActionResult Index(string userNickName, string tagName, int? page)
        {

            int currentPage = page ?? 1;
            if (page == 0)
            {
                currentPage = 1;
            }
            int onPage = 5;

                var wpis = _wpis.GetWpis();
                 var wpisList = wpis.ToList();

            if (string.IsNullOrEmpty(userNickName) && string.IsNullOrEmpty(tagName))
            {
                return View(wpisList.ToPagedList<Wpis>(currentPage, onPage));
            }
            else if (!string.IsNullOrEmpty(userNickName))
            {
                //get wpis by user id
                wpis = _wpis.GetWpisByUserNickName(userNickName).AsQueryable();
                wpisList = wpis.ToList();
            }
            else if (!string.IsNullOrEmpty(tagName))
            {
                wpis = _tag.getWpisWithSelectedTag(tagName).AsQueryable();
                wpisList = wpis.ToList();
            }
            return View(wpisList.ToPagedList<Wpis>(currentPage, onPage));
        }



        [HttpPost]
        [AuthorizeUserAttribute]
        [ValidateAntiForgeryToken]

        public ActionResult AddPlus(Wpis wpis)
        {
            var currentlyLoggedUserID = _user.GetIDOfCurrentlyLoggedUser().Value;
            var wpisToPlus = _wpis.GetWpisById(wpis.WpisID);

            bool checkWpisState = true;
            var IFWpisPlus =  _wpis.GetPlusWpis(wpisToPlus.WpisID, currentlyLoggedUserID);

            //prevent user from double plsuing
            if (wpisToPlus != null)
            {
                if (IFWpisPlus != null)
                {
                  checkWpisState = IFWpisPlus.IfPlusWpis;
                }
                else 
                {
                    var ifplus = new IfPlusowalWpis()
                    {
                        UserID = currentlyLoggedUserID,
                        WpisID = wpisToPlus.WpisID,
                        IfPlusWpis = false
                    };
                    _wpis.Add(ifplus);
                    _wpis.SaveChanges();
                    checkWpisState = ifplus.IfPlusWpis;
                }
            }

            if (wpisToPlus != null && User.Identity.Name != wpisToPlus.User.NickName && !checkWpisState)
            {
                wpisToPlus.Plusy = wpisToPlus.Plusy + 1;
                IFWpisPlus = _wpis.GetPlusWpis(wpisToPlus.WpisID, currentlyLoggedUserID);
                IFWpisPlus.IfPlusWpis = true;

               _wpis.UpdateOnlyPlusy(wpisToPlus);
               _wpis.UpdateIfWpisState(IFWpisPlus);
               _wpis.SaveChanges();

                var result = new { result = true, plusy = wpisToPlus.Plusy };
                return Json(result,
                            JsonRequestBehavior.AllowGet); ;
            }


            else if (wpisToPlus != null && User.Identity.Name != wpisToPlus.User.NickName && checkWpisState)
            {
                wpisToPlus.Plusy = wpisToPlus.Plusy - 1;
                IFWpisPlus = _wpis.GetPlusWpis(wpisToPlus.WpisID, currentlyLoggedUserID);
                IFWpisPlus.IfPlusWpis = false;
                _wpis.UpdateOnlyPlusy(wpisToPlus);
                _wpis.UpdateIfWpisState(IFWpisPlus);
                _wpis.SaveChanges();
                var result = new { result = false, plusy = wpisToPlus.Plusy };
                return Json( result , JsonRequestBehavior.AllowGet); 
            }
            else
            {
                var result = new { result = false, plusy = wpisToPlus.Plusy };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

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

//        [HttpPost]
//[AuthorizeUserAttribute]
//[ValidateAntiForgeryToken]
//public bool PlusujKurwo(int wpisId)
//        {
//            return this.AddPlus(wpisId);
            
//        }


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
        [AuthorizeUserAttribute]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Wpis/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [AuthorizeUserAttribute]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "WpisID,UserID,Content,AddingDate,Plusy")] Wpis wpis)
        {
            //tagz?

            wpis.AddingDate = DateTime.Now;
            wpis.EditingDate = null;
            var loggedUserId = _user.GetIDOfCurrentlyLoggedUser();
            wpis.UserID = loggedUserId.Value;


            if (ModelState.IsValid)
            {
                _wpis.Add(wpis);
                _wpis.SaveChanges();

                MatchCollection matches = Regex.Matches(wpis.Content, @"\B(\#[a-zA-Z0-9-,_]+\b)");
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

                    var wpisTag = new WpisTag()
                    {
                        TagID = tag.TagID,
                        WpisID = wpis.WpisID
                    };
                    _wpis.Add(wpisTag);
                    _wpis.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        // GET: Wpis/Edit/5
        [AuthorizeUserAttribute]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var IdOfCreator = _wpis.GetIdOfWpisCreator(id);
            //id of logged user
            int? currentlyLoggedUserId = _user.GetIDOfCurrentlyLoggedUser();
            if (currentlyLoggedUserId == null)
            {
                return RedirectToAction("LogIn", "User");
            }
            //end
            if (IdOfCreator == currentlyLoggedUserId)
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
        [AuthorizeUserAttribute]
        public ActionResult Edit([Bind(Include = "Content,WpisID")] Wpis wpis)
        {
            var logic = new HiddenLogic();
            wpis.AddingDate = _wpis.GetWpisWithAddDate(wpis);
            wpis.EditingDate = DateTime.Now;
            wpis.Plusy = 0;
            if (ModelState.IsValid)
            {
                //tutaj updejt contentu nastepuje 
                _wpis.UpdateContentAndPlusyAndEditDate(wpis);
                _wpis.SaveChanges();
                //old tags:
                IList<WpisTag> listaWpisTagsActual = _wpis.GetAllWpisTagsByWpisId(wpis.WpisID);
                List<string> listOfTagNames = new List<string>();
                foreach (var item in listaWpisTagsActual)
                {
                    listOfTagNames.Add(_tag.GetTagNamesByTagID(item.TagID));
                }
                //tu mam liste nazw tagow uzytych
                //tutej updejt tagow
                //new content here
                bool check = false;
                MatchCollection matches = Regex.Matches(wpis.Content, @"\B(\#[a-zA-Z0-9-,_]+\b)");
                //jak wpis ma wogole tagi to idz:
                if (wpis.WpisTags != null)
                {
                    foreach (var tag in matches)
                    {
                        if (listOfTagNames.Any(p => p.Contains(tag.ToString())))
                        {
                            logic.CheckTheDifferenceBetween(matches.Count, listOfTagNames.Count);
                            continue;
                        }
                        else
                        {
                            //sprawdz czy jest w bazie tagowej
                            //jak nie ma to dodaj i dodaj wpistag
                            //1. get tag from DB tags by tag name
                            var tagz = _tag.GetTagByName(tag.ToString());
                            // if no existing add so
                            if (tagz == null)
                            {
                                tagz = new Tag();
                                tagz.TagName = tag.ToString();
                                //id radnom
                                _tag.Add(tagz);
                                _tag.SaveChanges();
                                var wpisTag = new WpisTag()
                                {
                                    TagID = tagz.TagID,
                                    WpisID = wpis.WpisID
                                };
                                _wpis.Add(wpisTag);
                                _wpis.SaveChanges();
                            }
                            //jak nie ma w listOfTagNames ale jest w tagach bo uzyty gdzies indziej:
                            else
                            {
                                //remove wpistagStary po WpisID i TagID:
                                int tagID = tagz.TagID;
                                int wpisID = wpis.WpisID;
                                _tag.RemoveWpisTag(tagID, wpisID);
                                var wpisTag = new WpisTag()
                                {
                                    TagID = tagz.TagID,
                                    WpisID = wpis.WpisID
                                };
                                _wpis.Add(wpisTag);
                                _wpis.SaveChanges();
                            }
                        }
                    }
                    //jak nie ma nigdzie tagu ani uzytego ani nic to gerara
                    //check po to zeby ogarnac gdy po edicie nie zostaje nic zwiazanego z tagami
                    if (check == false && matches.Count != 0)
                    {
                        bool enterLoop = true;
                        //usuwaj z checklisty a nie z wpistagkowej bo jak 1 usuniesz a drugi zostawisz to lipa
                        foreach (var item in listaWpisTagsActual)
                        {
                            //get tag ID by match i usun pozostale?
                            //jak matchesTag jest w listofTagsActual to zostaw jak nie to gerara
                            foreach (var tag in matches)
                            {
                                var tagz = _tag.GetTagByName(tag.ToString());
                                //   var TagName =_tag.GetTagNamesByTagID(tagz.TagID);
                                //   var itemName = _tag.GetTagNamesByTagID(item.TagID);

                                if (item.TagID == tagz.TagID)
                                {
                                    enterLoop = false;
                                }
                                else
                                {
                                    enterLoop = true;
                                }

                                if (enterLoop == true)
                                {
                                    int tagID = item.TagID;
                                    int wpisID = wpis.WpisID;
                                    _tag.RemoveWpisTag(item.TagID, wpisID);
                                    //get list of ID 
                                    //remove wpistag
                                    //remove tag
                                    if (_tag.IfWpisOrCommentsHasTag(item.TagID))
                                    {
                                        _tag.RemoveTagsIfNotUsed(item.TagID);
                                        _tag.SaveChanges();
                                    }
                                }
                                enterLoop = true;
                            }
                        }
                    }
                    else if(check == false && matches.Count == 0)
                    {
                        foreach (var item in listaWpisTagsActual)
                        {
                            int wpisID = wpis.WpisID;
                            _tag.RemoveWpisTag(item.TagID, wpisID);
                            //get list of ID 
                            //remove wpistag
                            //remove tag
                            if (_tag.IfWpisOrCommentsHasTag(item.TagID))
                            {
                                _tag.RemoveTagsIfNotUsed(item.TagID);
                                _tag.SaveChanges();
                            }
                        }
                    }
                }
                //jak pusty wpis przed ale po juz nie
                else if (wpis.WpisTags == null && matches != null)
                {
                    foreach (var tag in matches)
                    {
                        var tagz = _tag.GetTagByName(tag.ToString());
                        // if no existing add so
                        if (tagz == null)
                        {
                            tagz = new Tag();
                            tagz.TagName = tag.ToString();
                            //id radnom
                            _tag.Add(tagz);
                            _tag.SaveChanges();
                            //tylko po dodaniu nowego tagu zmieni sie status tabeli wpistag?? WRONG
                            var wpisTag = new WpisTag()
                            {
                                TagID = tagz.TagID,
                                WpisID = wpis.WpisID
                            };
                            _wpis.Add(wpisTag);
                            _wpis.SaveChanges();
                        }
                        //jak nie ma w listOfTagNames ale jest w tagach bo uzyty gdzies indziej:
                        else
                        {
                            //remove wpistagStary po WpisID i TagID:
                            int tagID = tagz.TagID;
                            int wpisID = wpis.WpisID;
                            //to nie trzeba usuwac tylko dodac i elo - jak jest tag gdzies uzyty ale nie w tej edycji 

                            var wpisTag = new WpisTag()
                            {
                                TagID = tagz.TagID,
                                WpisID = wpis.WpisID
                            };
                            _wpis.Add(wpisTag);
                            _wpis.SaveChanges();
                        }
                    }
                    //jak nie ma nigdzie tagu ani uzytego ani nic to gerara
                    //check po to zeby ogarnac gdy po edicie nie zostaje nic zwiazanego z tagami
                    if (check == false)
                    {
                        foreach (var item in listaWpisTagsActual)
                        {
                            int wpisID = wpis.WpisID;
                            _tag.RemoveWpisTag(item.TagID, wpisID);
                            //get list of ID 
                            //remove wpistag
                            //remove tag
                            if (_tag.IfWpisOrCommentsHasTag(item.TagID))
                            {
                                _tag.RemoveTagsIfNotUsed(item.TagID);
                                _tag.SaveChanges();
                            }
                        }
                    }
                }

                return RedirectToAction("Index");
            }
            return View(wpis);
        }

        // GET: Wpis/Delete/5
        [AuthorizeUserAttribute]
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
        [AuthorizeUserAttribute]
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
        //custom authorize
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool isUserLogged = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            return isUserLogged;
        }
        //redirect to action 
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // Returns HTTP 401 - see comment in HttpUnauthorizedResult.cs.
            filterContext.Result = new RedirectToRouteResult(
                                       new RouteValueDictionary
                                       {
                                       { "action", "LogIn" },
                                       { "controller", "User" }
                                       });
        }
    }




    public class HiddenLogic
    {
        //metods that are repeaated

        public bool CheckTheDifferenceBetween(int var1, int var2)
        {
            bool check = true;
            if (var1 < var2)
            {
                check = false;
            }
            return check;
        }


        public void AddTagAfterEditing(Tag tag)
        {



        }

    }







}