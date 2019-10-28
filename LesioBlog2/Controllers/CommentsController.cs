using LesioBlog2_Repo.Abstract;
using LesioBlog2_Repo.Models;
using System;
using System.Collections.Generic;
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
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

        }

        // GET: Comments/Details/5
        public ActionResult Details(int? id)
        {

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // GET: Comments/Create
        public ActionResult Create()
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Gerarka hir");

        }
        [HttpPost]
        [AuthorizeUserAttribute]
        [ValidateAntiForgeryToken]

        public ActionResult AddPlus(Comment comment)
        {
            var currentlyLoggedUserID = _user.GetIDOfCurrentlyLoggedUser().Value;
            var commToPlus = _comm.GetCommentById(comment.CommentID);

            bool checkWpisState = true;
            var ifCommPlus= _comm.GetPlusComment(commToPlus.CommentID, currentlyLoggedUserID);

           // prevent user from double plsuing
            if (commToPlus != null)
            {
                if (ifCommPlus != null)
                {
                    checkWpisState = ifCommPlus.IfPlusWpis;
                }
                else
                {
                    var ifplus = new IfPlusowalComment()
                    {
                        UserID = currentlyLoggedUserID,
                        CommentID = commToPlus.CommentID,
                        IfPlusWpis = false
                    };
                         _comm.Add(ifplus);
                        _comm.SaveChanges();
                    checkWpisState = ifplus.IfPlusWpis;
                }
            }

            if (commToPlus != null && User.Identity.Name.ToLower() != commToPlus.User.NickName.ToLower() && !checkWpisState)
            {
                commToPlus.Plusy = commToPlus.Plusy + 1;
                ifCommPlus = _comm.GetPlusComment(commToPlus.CommentID, currentlyLoggedUserID);
                ifCommPlus.IfPlusWpis = true;
                _comm.UpdateOnlyPlusy(commToPlus);
                _comm.UpdateIfCommState(ifCommPlus);
                _comm.SaveChanges();

                var result = new { result = true, plusy = commToPlus.Plusy };
                return Json(result,
                            JsonRequestBehavior.AllowGet); ;
            }


            else if (commToPlus != null && User.Identity.Name.ToLower() != commToPlus.User.NickName.ToLower() && checkWpisState)
            {
                commToPlus.Plusy = commToPlus.Plusy - 1;
                ifCommPlus = _comm.GetPlusComment(commToPlus.CommentID, currentlyLoggedUserID);
                ifCommPlus.IfPlusWpis = false;
                _comm.UpdateOnlyPlusy(commToPlus);
                _comm.UpdateIfCommState(ifCommPlus);
                _comm.SaveChanges();
                var result = new { result = false, plusy = commToPlus.Plusy };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var result = new { result = false, plusy = commToPlus.Plusy };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

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
                        tag.TagName = tagName.ToString().ToLower();
                        //id radnom
                        _tag.Add(tag);
                        _tag.SaveChanges();
                    }

                    var commTag = new CommentTag()
                    {
                        TagID = tag.TagID,
                        CommentID = comment.CommentID
                    };
                    _comm.Add(commTag);
                    _comm.SaveChanges();
                }

                return RedirectToAction("Index", "Wpis");
            }
            return RedirectToAction("Index", "Wpis");
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
        public ActionResult Edit([Bind(Include = "CommentID,Content")] Comment comment)
        {
            var logic = new HiddenLogic();
            comment.Plusy = 0;
            comment.EditingDate = DateTime.Now;
            comment.AddingDate = _comm.GetCommentWithAddingDate(comment);

            if (ModelState.IsValid)
            {
                _comm.UpdateContentAndPlusyAndEditDate(comment);
                _comm.SaveChanges();

                //old tags:
                IList<CommentTag> listaCommentTagsActual = _comm.GetAllCommTagsByCommId(comment.CommentID);
                List<string> listOfTagNames = new List<string>();
                foreach (var item in listaCommentTagsActual)
                {
                    listOfTagNames.Add(_tag.GetTagNamesByTagID(item.TagID));
                }
                //tu mam liste nazw tagow uzytych
                //tutej updejt tagow
                //new content here
                bool check = false;
                MatchCollection matches = Regex.Matches(comment.Content, @"\B(\#[a-zA-Z0-9-,_]+\b)");
                //jak wpis ma wogole tagi to idz:
                if (comment.CommentTags != null)
                {
                    foreach (var tag in matches)
                    {
                        if (listOfTagNames.Any(p => p.Contains(tag.ToString().ToLower())))
                        {
                            logic.CheckTheDifferenceBetween(matches.Count, listOfTagNames.Count);
                            continue;
                        }
                        else
                        {
                            //sprawdz czy jest w bazie tagowej
                            //jak nie ma to dodaj i dodaj wpistag
                            //1. get tag from DB tags by tag name
                            var tagz = _tag.GetTagByName(tag.ToString().ToLower());
                            // if no existing add so
                            if (tagz == null)
                            {
                                tagz = new Tag();
                                tagz.TagName = tag.ToString().ToLower();
                                //id radnom
                                _tag.Add(tagz);
                                _tag.SaveChanges();
                                var commTag = new CommentTag()
                                {
                                    TagID = tagz.TagID,
                                    CommentID = comment.CommentID
                                };
                                _comm.Add(commTag);
                                _comm.SaveChanges();
                            }
                            //jak nie ma w listOfTagNames ale jest w tagach bo uzyty gdzies indziej:
                            else
                            {
                                //remove wpistagStary po WpisID i TagID:
                                int tagID = tagz.TagID;
                                int commID = comment.CommentID;
                                _tag.RemoveCommentTag(tagID, commID);
                                 var commTag = new CommentTag()
                                 {
                                     TagID = tagz.TagID,
                                     CommentID = comment.CommentID
                                 };
                                _comm.Add(commTag);
                                _comm.SaveChanges();
                            }
                        }
                    }
                    //jak nie ma nigdzie tagu ani uzytego ani nic to gerara
                    //check po to zeby ogarnac gdy po edicie nie zostaje nic zwiazanego z tagami
                    if (check == false && matches.Count != 0)
                    {
                        bool enterLoop = true;
                        //usuwaj z checklisty a nie z wpistagkowej bo jak 1 usuniesz a drugi zostawisz to lipa
                        foreach (var item in listaCommentTagsActual)
                        {
                            //get tag ID by match i usun pozostale?
                            //jak matchesTag jest w listofTagsActual to zostaw jak nie to gerara
                            foreach (var tag in matches)
                            {
                                var tagz = _tag.GetTagByName(tag.ToString().ToLower());
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
                                    int commID = comment.CommentID;
                                    _tag.RemoveCommentTag(tagID, commID);
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
                    else if (check == false && matches.Count == 0)
                    {
                        foreach (var item in listaCommentTagsActual)
                        {
                            int commID = comment.CommentID;
                            _tag.RemoveCommentTag(item.TagID, commID);
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
                else if (comment.CommentTags == null && matches != null)
                {
                    foreach (var tag in matches)
                    {
                        var tagz = _tag.GetTagByName(tag.ToString().ToLower());
                        // if no existing add so
                        if (tagz == null)
                        {
                            tagz = new Tag();
                            tagz.TagName = tag.ToString().ToLower();
                            //id radnom
                            _tag.Add(tagz);
                            _tag.SaveChanges();
                            //tylko po dodaniu nowego tagu zmieni sie status tabeli wpistag?? WRONG
                            var commTag = new CommentTag()
                            {
                                TagID = tagz.TagID,
                                CommentID = comment.CommentID
                            };
                            _comm.Add(commTag);
                            _comm.SaveChanges();
                            
                        }
                        //jak nie ma w listOfTagNames ale jest w tagach bo uzyty gdzies indziej:
                        else
                        {
                            //remove wpistagStary po WpisID i TagID:
                            int tagID = tagz.TagID;
                            int commID = comment.CommentID;
                            //to nie trzeba usuwac tylko dodac i elo - jak jest tag gdzies uzyty ale nie w tej edycji 

                            var commTag = new CommentTag()
                            {
                                TagID = tagID,
                                CommentID = commID
                            };
                            _comm.Add(commTag);
                            _comm.SaveChanges();
                        }
                    }
                    //jak nie ma nigdzie tagu ani uzytego ani nic to gerara
                    //check po to zeby ogarnac gdy po edicie nie zostaje nic zwiazanego z tagami
                    if (check == false)
                    {
                        foreach (var item in listaCommentTagsActual)
                        {
                            int commID = comment.CommentID;
                            _tag.RemoveCommentTag(item.TagID, commID);
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
