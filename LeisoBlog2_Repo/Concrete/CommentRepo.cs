using LesioBlog2_Repo.Abstract;
using LesioBlog2_Repo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;


namespace LesioBlog2_Repo.Concrete
{
    public class CommentRepo : ICommentRepo
    {
        private readonly IBlogContext _db;
        public CommentRepo(IBlogContext db)
        {
            this._db = db;
        }

        public void Add(Comment comment)
        {
            _db.Comments.Add(comment);
        }

        public void Add(CommentTag commTag)
        {
            _db.CommentTags.Add(commTag);
        }

         public void Add(IsCommUpvoted voteComm)
        {
            _db.IsCommUpvoted.Add(voteComm);
        }

        public void Delete(Comment comment)
        {
            //remove tags if are not used anywhere else
            var commTags = _db.CommentTags.Where(x => x.Comment_Id == comment.Comment_Id);
            //list of PostTags
            //Ilist because this allows to perform Savechanges in Foreach
            IList<CommentTag> listaforLoop = _db.CommentTags.Where(x => x.Comment_Id == comment.Comment_Id).ToList();
            foreach (var item in listaforLoop)
            {
                var tagID = item.Tag_Id;
                //get list of ID 
                _db.CommentTags.Remove(item);
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

            IList<IsCommUpvoted> lostOfIsCommUpvoted = _db.IsCommUpvoted
                .Where(x => x.Comment_Id == comment.Comment_Id)
                .ToList();
            foreach (var item in lostOfIsCommUpvoted)
            {
                _db.IsCommUpvoted.Remove(item);
                _db.SaveChanges();
            }
            _db.Comments.Remove(comment);
        }
        public Comment GetCommentById(int? id)
        {
            var comment = _db.Comments.Include(a=>a.User).Include(x=>x.CommentTags).FirstOrDefault(x => x.Comment_Id == id);
            if (comment == null)
            {
                return new Comment();
            }
            return comment;
        }

        public ICollection<Comment>  GetCommentByUserID(int? id)
        {
            var comment = _db.Comments.Where(x => x.User_Id == id).ToList();
            return comment;
        }

        public IsCommUpvoted GetPlusComment(int? idComment, int? idUser)
        {
            var plusedComment = _db.IsCommUpvoted.FirstOrDefault(x => x.Comment_Id == idComment && x.User_Id == idUser);
            if (plusedComment == null)
            {
                //null ref ex in controler OK
            }
            return plusedComment;
        }
        public List<Comment> GetCommentByUserNickName(string name)
        {
            var user = _db.Users.FirstOrDefault(x => x.NickName.ToLower() == name.ToLower());
            if (user != null)
            {
                var comments = _db.Comments.Include("Post").Include("User").Where(x => x.User_Id == user.User_Id);
                return comments.ToList();
            }
            else
            {
                return new List<Comment>();
            }
        }

        public DateTime GetCommentWithAddingDate(Comment comment)
        {
            DateTime data;
            int id = comment.Comment_Id;
            data = _db.Comments.AsNoTracking().FirstOrDefault(x => x.Comment_Id == id).AddingDate;
            if (data == null)
            {
                return new DateTime(); 
            }

            return data;
        }


        public void SaveChanges()
        {
            _db.SaveChanges();
        }


        public void UpdateContentAndPlusyAndEditDate(Comment comment)
        {
            _db.Comments.Attach(comment);
            _db.Entry(comment).Property("Content").IsModified = true;
            _db.Entry(comment).Property("Votes").IsModified = true;
            _db.Entry(comment).Property("EditingDate").IsModified = true;
        }
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

        public int GetIdOfCommentCreator(int? id)
        {
            int? ID = _db.Comments.FirstOrDefault(x => x.Comment_Id == id).User_Id;
            if (ID == null)
            {
                return 0;
            }
            return ID.GetValueOrDefault();
        }

        public void UpdateOnlyVotes(Comment comment)
        {
            _db.Comments.Attach(comment);
            _db.Entry(comment).Property("Votes").IsModified = true;
        }

        public void UpdateIfCommState(IsCommUpvoted isUpvoted)
        {
            _db.IsCommUpvoted.Attach(isUpvoted);
            _db.Entry(isUpvoted).Property("IsCommentUpvoted").IsModified = true;
        }
        public List<CommentTag> GetAllCommTagsByCommId(int? id)
        {
            var commTag = _db.CommentTags.Where(x => x.Comment_Id == id).ToList();
            return commTag;
        }
    }
}