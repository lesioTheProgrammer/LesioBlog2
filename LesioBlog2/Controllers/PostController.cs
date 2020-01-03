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
    public class PostController : Controller
    {
        private readonly IPostRepo _post;
        private readonly ITagRepo _tag;
        private readonly IUserRepo _user;

        public PostController(IPostRepo postrepo, ITagRepo tagrepo, IUserRepo user)
        {
            this._post = postrepo;
            this._tag = tagrepo;
            this._user = user;
        }
        // GET: Post
        public ActionResult Index(string userNickName, string tagName, int? page, string userCommentNickname)
        {
            int currentPage = page ?? 1;
            if (page == 0)
            {
                currentPage = 1;
            }
            int onPage = 5;
            var post = _post.GetPost();

            var postList = post.OrderByDescending(x=>x.AddingDate).ToList();
            if (string.IsNullOrEmpty(userNickName) && string.IsNullOrEmpty(tagName) && string.IsNullOrEmpty(userCommentNickname))
            {
                return View(postList.ToPagedList<Post>(currentPage, onPage));
            }
            else if (!string.IsNullOrEmpty(userNickName))
            {
                //get post by user name
                post = _post.GetPostByUsrNickName(userNickName).AsQueryable();
                postList = post.ToList();
            }
            else if (!string.IsNullOrEmpty(tagName))
            {
                post = _tag.getWpisWithSelectedTag(tagName).AsQueryable();
                postList = post.ToList();
            }
            else if (!string.IsNullOrEmpty(userCommentNickname))
            {
                //get post cointating commentName
                post = _post.GetPostCointaininCommWithNickname(userCommentNickname).AsQueryable();
                postList = post.ToList();
            }
            return View(postList.ToPagedList<Post>(currentPage, onPage));
        }


        [HttpPost]
        [AuthorizeUserAttribute]
        [ValidateAntiForgeryToken]
        public ActionResult AddPlus(Post post)
        {
            var currentlyLoggedUserID = _user.GetIDOfCurrentlyLoggedUser().Value;
            var postToUpvote = _post.GetPostByID(post.Post_Id);

            bool checkPostState = true;
            var postIsUpvoted =  _post.GetUpvPost(postToUpvote.Post_Id, currentlyLoggedUserID);

            //prevent user from double plsuing
            if (postToUpvote != null)
            {
                if (postIsUpvoted != null)
                {
                  checkPostState = postIsUpvoted.IsPostUpvoted;
                }
                else 
                {
                    var postIsUpvtd = new IsPostUpvd()
                    {
                        User_Id = currentlyLoggedUserID,
                        Post_Id = postToUpvote.Post_Id,
                        IsPostUpvoted = false
                    };
                    _post.Add(postIsUpvtd);
                    _post.SaveChanges();
                    checkPostState = postIsUpvtd.IsPostUpvoted;
                }
            }

            if (postToUpvote != null && User.Identity.Name.ToLower() != postToUpvote.User.NickName.ToLower() && !checkPostState)
            {
                postToUpvote.Votes = postToUpvote.Votes + 1;
                postIsUpvoted = _post.GetUpvPost(postToUpvote.Post_Id, currentlyLoggedUserID);
                postIsUpvoted.IsPostUpvoted = true;

               _post.UpdateOnlyVotes(postToUpvote);
               _post.UpdateIsVotedState(postIsUpvoted);
               _post.SaveChanges();

                var result = new { result = true, votes = postToUpvote.Votes };
                return Json(result,
                            JsonRequestBehavior.AllowGet); ;
            }


            else if (postToUpvote != null && User.Identity.Name.ToLower() != postToUpvote.User.NickName.ToLower() && checkPostState)
            {
                postToUpvote.Votes = postToUpvote.Votes - 1;
                postIsUpvoted = _post.GetUpvPost(postToUpvote.Post_Id, currentlyLoggedUserID);
                postIsUpvoted.IsPostUpvoted = false;
                _post.UpdateOnlyVotes(postToUpvote);
                _post.UpdateIsVotedState(postIsUpvoted);
                _post.SaveChanges();
                var result = new { result = false, votes = postToUpvote.Votes };
                return Json( result , JsonRequestBehavior.AllowGet); 
            }
            else
            {
                var result = new { result = false, votes = postToUpvote.Votes };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GoToParentWpis(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var wpis = _post.GetPostByID(id);
            return View(wpis);
        }


        // GET: Post/Details/5
        [AuthorizeUserAttribute]
        public ActionResult Details(int? id)
        {

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Not avialable");
        }

        // GET: Post/Create
        [AuthorizeUserAttribute]
        public ActionResult Create()
        {
           return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // POST: Post/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [AuthorizeUserAttribute]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Post_Id,User_Id,Content,AddingDate,Votes")] Post post)
        {
            post.AddingDate = DateTime.Now;
            post.EditingDate = null;
            var loggedUserId = _user.GetIDOfCurrentlyLoggedUser();
            post.User_Id = loggedUserId.Value;


            if (ModelState.IsValid)
            {
                _post.Add(post);
                _post.SaveChanges();

                MatchCollection matches = Regex.Matches(post.Content, @"\B(\#[a-zA-Z0-9-,_]+\b)");
                //postId first important
                foreach (var tagName in matches)
                {
                    var tag = _tag.GetTagByName(tagName.ToString().ToLower());
                    if (tag == null)
                    {
                        tag = new Tag();
                        tag.TagName = tagName.ToString().ToLower();
                        _tag.Add(tag);
                        _tag.SaveChanges();
                    }

                    var postTag = new PostTag()
                    {
                        Tag_Id = tag.Tag_Id,
                        Post_Id = post.Post_Id
                    };
                    _post.Add(postTag);
                    _post.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        // GET: Post/Edit/5
        [AuthorizeUserAttribute]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var IdOfCreator = _post.GetIdOpfPostCreator(id);
            //id of logged user
            int? currentlyLoggedUserId = _user.GetIDOfCurrentlyLoggedUser();
            if (currentlyLoggedUserId == null)
            {
                return RedirectToAction("LogIn", "User");
            }
            //end
            if (IdOfCreator == currentlyLoggedUserId)
            {
                Post wpis = _post.GetPostByID(id);
                if (wpis == null)
                {
                    return HttpNotFound();
                }
                return View(wpis);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "You can't edit someone else post, leave");
            }

        }

        // POST: Post/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUserAttribute]
        public ActionResult Edit([Bind(Include = "Content,Post_Id")] Post post)
        {
            var logic = new HiddenLogic();
            post.AddingDate = _post.GetPostWithAddingDate(post);
            post.EditingDate = DateTime.Now;
            post.Votes = 0;
            if (ModelState.IsValid)
            {
                //content updated here
                _post.UpdateContentUpvotesAddDate(post);
                _post.SaveChanges();
                //old tags:
                IList<PostTag> listPostTagsActual = _post.GetAllPostTagsByPostID(post.Post_Id);
                List<string> listOfTagNames = new List<string>();
                foreach (var item in listPostTagsActual)
                 {
                    listOfTagNames.Add(_tag.GetTagNamesByTagID(item.Tag_Id));
                }
                //list of tags used
                //tags updated
                //new content here
                bool check = false;
                bool removalCheck = true;
                MatchCollection matches = Regex.Matches(post.Content, @"\B(\#[a-zA-Z0-9-,_]+\b)");
                //if post has tags
                if (post.PostTags != null)
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
                            AddTagAndPostTag(post, tag, removalCheck);
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
                        foreach (var item in listPostTagsActual)
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
                                    if (_tag.CheckIfPostTagExist(item.Tag_Id, post.Post_Id))
                                    {
                                        _tag.RemovePostTag(item.Tag_Id, post.Post_Id);
                                        //get list of ID , remove postTag, remove tag 
                                        if (_tag.IfPostOrCommentHaveTags(item.Tag_Id))
                                        {
                                            _tag.RemoveTagsIfNotUsed(item.Tag_Id);
                                            _tag.SaveChanges();
                                        }
                                    }
                                }
                                enterLoop = true;
                            }
                        }
                    }
                    else if(check == false && matches.Count == 0)
                    {
                        RemovePostandPostTag(post, listPostTagsActual);
                    }
                }
                //if post is empty before and after is not
                else if (post.PostTags == null && matches != null)
                {
                    removalCheck = false;
                    foreach (var tag in matches)
                    {
                        AddTagAndPostTag(post, tag, removalCheck);
                    }
                    removalCheck = true;
                    //if there isnt any tag used, bool check if after edtiting theres nothing relative to tags
                    if (check == false)
                    {
                        RemovePostandPostTag(post, listPostTagsActual);
                    }
                }

                return RedirectToAction("Index");
            }
            return View(post);
        }

        private void RemovePostandPostTag(Post post, IList<PostTag> listPostTagsActual)
        {
            foreach (var item in listPostTagsActual)
            {
                _tag.RemovePostTag(item.Tag_Id, post.Post_Id);
                //get list of ID 
                //remove postTag
                //remove tag
                if (_tag.IfPostOrCommentHaveTags(item.Tag_Id))
                {
                    _tag.RemoveTagsIfNotUsed(item.Tag_Id);
                    _tag.SaveChanges();
                }
            }
        }

        private void AddTagAndPostTag(Post post, object tag, bool removalCheck)
        {
            //check if exist in tagList, add tag and postTag if dont exsist, 1. get tag from DB tags by tag name
            var tagz = _tag.GetTagByName(tag.ToString().ToLower());
            // if no existing add 
            if (tagz == null)
            {
                tagz = new Tag();
                tagz.TagName = tag.ToString().ToLower();
                _tag.Add(tagz);
                _tag.SaveChanges();
                var postTag = new PostTag()
                {
                    Tag_Id = tagz.Tag_Id,
                    Post_Id = post.Post_Id
                };
                _post.Add(postTag);
                _post.SaveChanges();
            }
            //null in listOfTagNames but exist in tags because used somewhere elese
            else
            {
                //remove postTag by Post_Id i Tag_Id:
                if (removalCheck)
                {
                _tag.RemovePostTag(tagz.Tag_Id, post.Post_Id);
                }
                var postTag = new PostTag()
                {
                    Tag_Id = tagz.Tag_Id,
                    Post_Id = post.Post_Id
                };
                _post.Add(postTag);
                _post.SaveChanges();
            }
        }

        // GET: Post/Delete/5
        [AuthorizeUserAttribute]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            bool isUserLogged = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            var IdOfCreator = _post.GetIdOpfPostCreator(id);
            //id of logged user
            int? currentlyLoggedUserId = _user.GetIDOfCurrentlyLoggedUser();
            if (currentlyLoggedUserId == null)
            {
                return RedirectToAction("LogIn", "User");
            }
            //end
            if (isUserLogged && IdOfCreator == currentlyLoggedUserId)
            {
                Post post = _post.GetPostByID(id);
                if (post == null)
                {
                    return HttpNotFound();
                }
                return View(post);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "You can't DELETE someone else post");
            }
        }

        // POST: Post/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeUserAttribute]
        public ActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = _post.GetPostByID(id);

            _post.Delete(post);
            _post.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _post.Dispose();
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