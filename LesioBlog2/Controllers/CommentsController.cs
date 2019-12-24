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
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Not avialable");

        }
        [HttpPost]
        [AuthorizeUserAttribute]
        [ValidateAntiForgeryToken]
        public ActionResult AddPlus(Comment comment)
        {
            var currentlyLoggedUserID = _user.GetIDOfCurrentlyLoggedUser().Value;
            var commToVote = _comm.GetCommentById(comment.Comment_Id);

            bool checkPostState = true;
            var commToVoteItem = _comm.GetPlusComment(commToVote.Comment_Id, currentlyLoggedUserID);
            // prevent user from double voting
            if (commToVote != null)
            {
                if (commToVoteItem != null)
                {
                    checkPostState = commToVoteItem.IsCommentUpvoted;
                }
                else
                {
                    var commVoteEntity = new IsCommUpvoted()
                    {
                        User_Id = currentlyLoggedUserID,
                        Comment_Id = commToVote.Comment_Id,
                        IsCommentUpvoted = false
                    };
                    _comm.Add(commVoteEntity);
                    _comm.SaveChanges();
                    checkPostState = commVoteEntity.IsCommentUpvoted;
                }
            }

            if (commToVote != null && User.Identity.Name.ToLower() != commToVote.User.NickName.ToLower() && !checkPostState)
            {
                commToVote.Votes = commToVote.Votes + 1;
                commToVoteItem = _comm.GetPlusComment(commToVote.Comment_Id, currentlyLoggedUserID);
                commToVoteItem.IsCommentUpvoted = true;
                _comm.UpdateOnlyVotes(commToVote);
                _comm.UpdateIfCommState(commToVoteItem);
                _comm.SaveChanges();

                var result = new { result = true, votes = commToVote.Votes };
                return Json(result,
                            JsonRequestBehavior.AllowGet); ;
            }


            else if (commToVote != null && User.Identity.Name.ToLower() != commToVote.User.NickName.ToLower() && checkPostState)
            {
                commToVote.Votes = commToVote.Votes - 1;
                commToVoteItem = _comm.GetPlusComment(commToVote.Comment_Id, currentlyLoggedUserID);
                commToVoteItem.IsCommentUpvoted = false;
                _comm.UpdateOnlyVotes(commToVote);
                _comm.UpdateIfCommState(commToVoteItem);
                _comm.SaveChanges();
                var result = new { result = false, votes = commToVote.Votes };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var result = new { result = false, votes = commToVote.Votes };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }
        // POST: Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Post_Id,Content,User_Id,ID,AddingDate,Votes,EditingDate")] Comment comment)
        {
            comment.Votes = 0;
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
                comment.User_Id = nullableUserID.GetValueOrDefault();
            }
            comment.EditingDate = null;

            var result = new { result = true };
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
                        Tag_Id = tag.Tag_Id,
                        Comment_Id = comment.Comment_Id
                    };
                    _comm.Add(commTag);
                    _comm.SaveChanges();
                }
                return RedirectToAction("Index", "Post");
            }
            return RedirectToAction("Index", "Post");
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
        public ActionResult Edit([Bind(Include = "Comment_Id,Content")] Comment comment)
        {
            var logic = new HiddenLogic();
            comment.Votes = 0;
            comment.EditingDate = DateTime.Now;
            comment.AddingDate = _comm.GetCommentWithAddingDate(comment);

            if (ModelState.IsValid)
            {
                _comm.UpdateContentAndPlusyAndEditDate(comment);
                _comm.SaveChanges();

                //old tags:
                IList<CommentTag> listaCommentTagsActual = _comm.GetAllCommTagsByCommId(comment.Comment_Id);
                List<string> listOfTagNames = new List<string>();
                foreach (var item in listaCommentTagsActual)
                {
                    listOfTagNames.Add(_tag.GetTagNamesByTagID(item.Tag_Id));
                }
                //list of name tags used
                //update of tags
                //new content here
                bool check = false;
                MatchCollection matches = Regex.Matches(comment.Content, @"\B(\#[a-zA-Z0-9-,_]+\b)");
                //if comment has tags ->enter
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
                            RemovecommentAndCommentTags(comment, tag);
                        }
                    }
                    //if there isnt any tag used
                    //bool check if after edtiting theres nothing relative to tags
                    if (check == false && matches.Count != 0)
                    {
                        bool enterLoop = true;
                        var listOfMatches = new List<Tag>();
                        foreach (var tagFake in matches)
                        {
                            var tag2 = _tag.GetTagByName(tagFake.ToString());
                            listOfMatches.Add(tag2);
                        }
                        //remove from checklist, not postTags (if you try to remove only 1 and you will keep the second --failure)
                        foreach (var item in listaCommentTagsActual)
                        {
                            //get tagID by match and remove the rest
                            //if matchesTags exists in listOfTagsActual - leave it;
                            //new list of tags
                            foreach (var tag in listOfMatches)
                            {
                                if (listOfMatches.Any(x => x.Tag_Id == item.Tag_Id))
                                {
                                    enterLoop = false;
                                }
                                else
                                {
                                    enterLoop = true;
                                }

                                if (enterLoop == true)
                                {
                                    if (_tag.CheckIfCommTagExist(item.Tag_Id, comment.Comment_Id))
                                    {
                                        RemovecommentTag(item, comment.Comment_Id);
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
                            RemovecommentTag(item, comment.Comment_Id);
                        }
                    }
                }
                //if post is empty before and after is not
                else if (comment.CommentTags == null && matches != null)
                {
                    foreach (var tag in matches)
                    {
                        RemovecommentAndCommentTags(comment,  tag);
                    }
                    //if there isnt any tag used
                    //bool check if after edtiting theres nothing relative to tags
                    if (check == false)
                    {
                        foreach (var item in listaCommentTagsActual)
                        {
                            RemovecommentTag(item, comment.Comment_Id);
                        }
                    }
                }
                return RedirectToAction("Index", "Post");
            }
            return View(comment);
        }

        private void RemovecommentTag(CommentTag item, int commID)
        {
            _tag.RemoveCommentTag(item.Tag_Id, commID);
            //get list of ID 
            //remove postTag
            //remove tag
            if (_tag.IfPostOrCommentHaveTags(item.Tag_Id))
            {
                _tag.RemoveTagsIfNotUsed(item.Tag_Id);
                _tag.SaveChanges();
            }
        }

        private void RemovecommentAndCommentTags(Comment comment, object tag)
        {
            //check if exist in tag base
            //if not add tag and postTag
            //1. get tag from DB tags by tag name
            var tagz = _tag.GetTagByName(tag.ToString().ToLower());
            // if no existing add it
            if (tagz == null)
            {
                tagz = new Tag();
                tagz.TagName = tag.ToString().ToLower();
                //id radnom
                _tag.Add(tagz);
                _tag.SaveChanges();
                var commTag = new CommentTag()
                {
                    Tag_Id = tagz.Tag_Id,
                    Comment_Id = comment.Comment_Id
                };
                _comm.Add(commTag);
                _comm.SaveChanges();
            }
            //if not existing in listOfTagNames but used somewhere else:
            else
            {
                //remove removeCommVote (get by tagID and CommID)
                int tagID = tagz.Tag_Id;
                int commID = comment.Comment_Id;
                _tag.RemoveCommentTag(tagID, commID);
                var commTag = new CommentTag()
                {
                    Tag_Id = tagz.Tag_Id,
                    Comment_Id = comment.Comment_Id
                };
                _comm.Add(commTag);
                _comm.SaveChanges();
            }
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
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "You can't DELETE someone else comment");
            }
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeUserAttribute]
        public ActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = _comm.GetCommentById(id);
            _comm.Delete(comment);
            _comm.SaveChanges();
            return RedirectToAction("Index", "Post");
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
