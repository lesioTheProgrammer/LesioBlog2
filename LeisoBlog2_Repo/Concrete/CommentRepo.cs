using LeisoBlog2_Repo.Abstract;
using LesioBlog2_Repo.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LeisoBlog2_Repo.Concrete
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

        public void Delete(Comment comment)
        {

            _db.Comments.Remove(comment);
        }

      

        public Comment GetCommentById(int? id)
        {
            var comment = _db.Comments.SingleOrDefault(x => x.ID == id);
            return comment;
        }

        public ICollection<Comment>  GetCommentByUserID(int? id)
        {
            var comment = _db.Comments.Where(x => x.UserID == id).Select(x=>x);

            var list = comment.ToList();

            return list;

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

            int id = comment.ID;

            data = _db.Comments.AsNoTracking().SingleOrDefault(x => x.ID == id).AddingDate;


            return data;
        }

        public void SaveChanges()
        {
            _db.SaveChanges();
        }

        public void Update(Comment comment)
        {
            _db.Entry(comment).State = System.Data.Entity.EntityState.Modified;
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

    }
}