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
            var commTags = _db.CommentTags.Where(x => x.CommentID == comment.CommentID);
            //lista wpistagow'
            //Ilist because this allows to perform Savechanges in Foreach
            IList<CommentTag> listaforLoop = _db.CommentTags.Where(x => x.CommentID == comment.CommentID).ToList();


            foreach (var item in listaforLoop)
            {
                var tagID = item.TagID;
                //get list of ID 
                _db.CommentTags.Remove(item);
                //save changes
                _db.SaveChanges();
                if (!_db.CommentTags.Any(x => x.TagID == tagID) && !_db.WpisTags.Any(x => x.TagID == tagID))
                {
                    var tagToRemove = _db.Tags.FirstOrDefault(x => x.TagID == tagID);
                    if (tagToRemove == null)
                    {
                        break; //???
                    }

                    _db.Tags.Remove(tagToRemove);
                }
            }

            IList<IfPlusowalComment> listaifPlusComm = _db.IfPlusowalComment
                .Where(x => x.CommentID == comment.CommentID)
                .ToList();

            foreach (var item in listaifPlusComm)
            {
                _db.IfPlusowalComment.Remove(item);
                _db.SaveChanges();
            }

            _db.Comments.Remove(comment);
        }

      

        public Comment GetCommentById(int? id)
        {
            var comment = _db.Comments.Include(a=>a.User).Include(x=>x.CommentTags).FirstOrDefault(x => x.CommentID == id);
            if (comment == null)
            {
                return new Comment();
            }
            return comment;
        }

        public ICollection<Comment>  GetCommentByUserID(int? id)
        {
            var comment = _db.Comments.Where(x => x.UserID == id).ToList();
            return comment;
        }

        public IfPlusowalComment GetPlusComment(int? idComment, int? idUser)
        {
            var plusedComment = _db.IfPlusowalComment.FirstOrDefault(x => x.CommentID == idComment && x.UserID == idUser);
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

            data = _db.Comments.AsNoTracking().FirstOrDefault(x => x.CommentID == id).AddingDate;
            if (data == null)
            {
                return new DateTime(); 
            }

            return data;
        }


        public Comment FindCommentByID(int? id)
        {
            var comment = _db.Comments.FirstOrDefault(x => x.CommentID == id);
            if (comment == null)
            {
                return new Comment();
            }
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
            int? ID = _db.Comments.FirstOrDefault(x => x.CommentID == id).UserID;
            if (ID == null)
            {
                return 0;
            }
            return ID.GetValueOrDefault();
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




        public List<CommentTag> GetAllCommTagsByCommId(int? id)
        {

            var returning = _db.CommentTags.Where(x => x.CommentID == id).ToList();
            return returning;
        }
    }
}