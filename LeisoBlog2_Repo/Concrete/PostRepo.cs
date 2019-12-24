using LesioBlog2_Repo.Abstract;
using LesioBlog2_Repo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace LesioBlog2_Repo.Concrete
{
    public class PostRepo : IPostRepo, IDisposable
    {

        private readonly IBlogContext _db;

        public PostRepo(IBlogContext db)
        {
            this._db = db;
        }

        public object Database => _db.Database;


        public void Add(Post post)
        {
            _db.Post.Add(post);
        }

        public void Add(PostTag postTag)
        {
            _db.PostTags.Add(postTag);
        }
        public void Add(IsPostUpvd isUpvoted)
        {
            _db.IsPostUpvd.Add(isUpvoted);
        }

        public IsPostUpvd GetUpvPost(int? idPost, int? idUser)
        {
            var upvotedPost = _db.IsPostUpvd
                .FirstOrDefault(x => x.Post_Id == idPost && x.User_Id == idUser);
            //null ref ex in controller OK
            return upvotedPost;
        }

        public void Delete(Post post)
        {
            //remove comments proper way
            #region
            var commentsChildren = _db.Comments.Where(x => x.Post_Id == post.Post_Id);
            var commentChildenlist = commentsChildren.ToList(); // to avoid dataReader excep - caused by iterating on IQueryable
            foreach (var item in commentChildenlist)
            {
                //remove comments
                var commTags = _db.CommentTags.Where(x => x.Comment_Id == item.Comment_Id);
                //listofPostTags
                //Ilist because this allows to perform Savechanges in Foreach
                IList<CommentTag> listaCommentLoop = _db.CommentTags.Where(x => x.Comment_Id == item.Comment_Id).ToList();
                foreach (var item2 in listaCommentLoop)
                {
                    var tagID = item2.Tag_Id;
                    //get list of ID 
                    _db.CommentTags.Remove(item2);
                    //save changes
                    _db.SaveChanges();
                    if (!_db.CommentTags.Any(x => x.Tag_Id == tagID) && !_db.PostTags.Any(x => x.Tag_Id == tagID))
                    {
                        var tagToRemove = _db.Tags.FirstOrDefault(x => x.Tag_Id == tagID);
                        if (tagToRemove == null)
                        {
                            throw new FieldAccessException();
                        }
                        _db.Tags.Remove(tagToRemove);
                    }
                }
                IList<IsCommUpvoted> listaifPlusComm = _db.IsCommUpvoted
                .Where(x => x.Comment_Id == item.Comment_Id).ToList();

                foreach (var item3 in listaifPlusComm)
                {
                    _db.IsCommUpvoted.Remove(item3);
                    _db.SaveChanges();
                }
                _db.Comments.Remove(item);
            }
            #endregion
            //remove tags if are not used anywhere else
            var wpisTags = _db.PostTags.Where(x => x.Post_Id == post.Post_Id);
            //listofPostTags
            //Ilist because this allows to perform Savechanges in Foreach
            IList<PostTag> listaforLoop = _db.PostTags.Where(x => x.Post_Id == post.Post_Id).ToList();
            foreach (var item in listaforLoop)
            {
                var tagID = item.Tag_Id;
                //get list of ID 
                _db.PostTags.Remove(item);
                //save changes
                _db.SaveChanges();
                if (!_db.PostTags.Any(x=>x.Tag_Id == tagID) && !_db.CommentTags.Any(x => x.Tag_Id == tagID))
                {
                    var tagToRemove = _db.Tags.FirstOrDefault(x => x.Tag_Id == tagID);
                    if (tagToRemove == null)
                    {
                        break; //???
                    }
                    _db.Tags.Remove(tagToRemove);
                }
            }
            IList<IsPostUpvd> listIfPostUpvd = _db.IsPostUpvd
              .Where(x => x.Post_Id == post.Post_Id).ToList();
            foreach (var item in listIfPostUpvd)
            {
                _db.IsPostUpvd.Remove(item);
                _db.SaveChanges();
            }
            _db.Post.Remove(post);
        }
        //disposing- garb collecting
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                  _db.Dispose();
                }
            }
            disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public IQueryable<Post> GetPost()
        {
            var listofWpis = _db.Post.Include(x => x.Comments).Include(x => x.User).Include(x=>x.Comments.Select(u=>u.User));
            return listofWpis;
        }

        public Post GetPostByID(int? id)
        {
            var wpis = _db.Post.Include(x=>x.User).Include(x => x.PostTags).Include(x => x.Comments.Select(c=>c.User)).FirstOrDefault(x => x.Post_Id == id);
         
            return wpis;
        }

        public ICollection<Post> GetPostByUserID(int? id)
        {
            var wpis = _db.Post.Where(x => x.User_Id == id).ToList();
            return wpis;
        }

        public List<Post> GetPostByUsrNickName(string name)
        {
            var user = _db.Users.FirstOrDefault(x => x.NickName.ToLower() == name.ToLower());
            if (user != null)
            {
                var wpis = _db.Post.Where(x => x.User_Id == user.User_Id).Include(x=>x.Comments.Select(u=>u.User)).Include(x=>x.User).Select(x=>x).ToList();
                return wpis;
            }
            //else:
            return new List<Post>();
        }

        public List<Post> GetPostCointaininCommWithNickname(string name)
        {
            var user = _db.Users.FirstOrDefault(x => x.NickName.ToLower() == name.ToLower());
            if (user != null)
            {
                var wpis = _db.Post
                    .Where (x=>x.Comments.Any(c=>c.User_Id== user.User_Id))
                    .Include(x => x.Comments.Select(u => u.User))
                    .Include(x => x.User)
                    .ToList();
                return wpis;

            }
            return new List<Post>();
        }
        public int GetIdOpfPostCreator(int? id)
        {
            int? wpisUserId = _db.Post.FirstOrDefault(x => x.Post_Id == id).User_Id;
            return wpisUserId.GetValueOrDefault();
        }
        public DateTime GetPostWithAddingDate(Post post)
        {
            DateTime data;
            //first post, later post add date
            int id = post.Post_Id;
            //AsNoTracking to drop conn with db (otherwise Id have 2 posts and only one changed)
            data = _db.Post
                .AsNoTracking()
                .SingleOrDefault(x=>x.Post_Id == id).AddingDate;
            if (data == null)
            {
                data = DateTime.Now;
            }
            return data;
        }


        public List<PostTag> GetAllPostTagsByPostID(int? id)
        {

            var postTag = _db.PostTags.Where(x => x.Post_Id == id).ToList();
            if (postTag == null)
            {
                return new List<PostTag>();
            }
            return postTag;
        }
        public void SaveChanges()
        {
            _db.SaveChanges();
        }


        public void UpdateContentUpvotesAddDate(Post post)
        {
            _db.Post.Attach(post);
            _db.Entry(post).Property("Content").IsModified = true;
            _db.Entry(post).Property("Votes").IsModified = true;
            _db.Entry(post).Property("EditingDate").IsModified = true;
        }

        public void UpdateOnlyVotes(Post post)
        {
            _db.Post.Attach(post);
            _db.Entry(post).Property("Votes").IsModified = true;
        }

        public void UpdateIsVotedState(IsPostUpvd isUpvoted)
        {
            _db.IsPostUpvd.Attach(isUpvoted);
            _db.Entry(isUpvoted).Property("IsPostUpvoted").IsModified = true;
        }
    }
}