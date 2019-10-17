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

         public void Add(IfPlusowalComment plusComm)
        {
            _db.IfPlusowalComment.Add(plusComm);
        }



        public void Delete(Comment comment)
        {
            //remove tags if are not used anywhere else
            var commTags = _db.CommentTags.Where(x => x.CommentID == comment.CommentID).Select(x => x);
            //lista wpistagow'
            //Ilist because this allows to perform Savechanges in Foreach
            IList<CommentTag> listaforLoop = _db.CommentTags.Where(x => x.CommentID == comment.CommentID)
                .Select(x => x)
                .ToList();

            foreach (var item in listaforLoop)
            {
                var tagID = item.TagID;
                //get list of ID 
                _db.CommentTags.Remove(item);
                //save changes
                _db.SaveChanges();
                if (!_db.CommentTags.Any(x => x.TagID == tagID) && !_db.WpisTags.Any(x => x.TagID == tagID))
                {
                    var tagToRemove = _db.Tags.Where(x => x.TagID == tagID).SingleOrDefault();
                    _db.Tags.Remove(tagToRemove);
                }
            }

            _db.Comments.Remove(comment);
        }

      

        public Comment GetCommentById(int? id)
        {
            var comment = _db.Comments.Include(a=>a.User).SingleOrDefault(x => x.CommentID == id);
            return comment;
        }

        public ICollection<Comment>  GetCommentByUserID(int? id)
        {
            var comment = _db.Comments.Where(x => x.UserID == id).Select(x=>x);

            var list = comment.ToList();

            return list;

        }

        public IfPlusowalComment GetPlusComment(int? idComment, int? idUser)
        {
            var plusedComment = _db.IfPlusowalComment
                .Where(x => x.CommentID == idComment)
                .Where(x => x.UserID == idUser)
                .SingleOrDefault();
            return plusedComment;
        }



        public List<Comment> GetCommentByUserNickName(string name)
        {
            var user = _db.Users.SingleOrDefault(x => x.NickName.ToLower() == name.ToLower());
            if (user != null)
            {
                var comments = _db.Comments.Include("Wpis").Include("User").Where(x => x.UserID == user.UserID);
                return comments.ToList();
            }
            else
            {
                return new List<Comment>();
            }

        }

        public IQueryable<Comment> GetComment()
        {
            var comments = _db.Comments.Include("User");
            return comments;
        }

        public IQueryable<Comment> GetPages(int? page, int? pagesize)
        {
            throw new NotImplementedException();
        }

        public DateTime GetCommentWithAddingDate(Comment comment)
        {

            DateTime data;

            int id = comment.CommentID;

            data = _db.Comments.AsNoTracking().SingleOrDefault(x => x.CommentID == id).AddingDate;


            return data;
        }


        public Comment FindCommentByID(int? id)
        {
            var comment = _db.Comments.SingleOrDefault(x => x.CommentID == id);
            return comment;
        }


        public void SaveChanges()
        {
            _db.SaveChanges();
        }

        public void Update(Comment comment)
        {
            _db.Entry(comment).State = System.Data.Entity.EntityState.Modified;
        }

        public void UpdateContentAndPlusyAndEditDate(Comment comment)
        {
            _db.Comments.Attach(comment);
            _db.Entry(comment).Property("Content").IsModified = true;
            _db.Entry(comment).Property("Plusy").IsModified = true;
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
            var ID = _db.Comments.SingleOrDefault(x => x.CommentID == id).UserID;
            return ID;
        }



        public void UpdateOnlyPlusy(Comment comment)
        {
            _db.Comments.Attach(comment);
            _db.Entry(comment).Property("Plusy").IsModified = true;
        }

        public void UpdateIfCommState(IfPlusowalComment ifplus)
        {
            _db.IfPlusowalComment.Attach(ifplus);
            _db.Entry(ifplus).Property("IfPlusWpis").IsModified = true;
        }


    }
}