using LesioBlog2_Repo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeisoBlog2_Repo.Abstract
{
    public interface ICommentRepo : IDisposable
    {
        IQueryable<Comment> GetComment();
        List<Comment> GetCommentByUserNickName(string name);
        Comment GetCommentById(int? id);
        void SaveChanges();
        void Add(Comment comment);
        void Update(Comment comment);
        IQueryable<Comment> GetPages(int? page, int? pagesize);

        DateTime GetCommentWithAddingDate(Comment comment);

        ICollection<Comment> GetCommentByUserID(int? id);


        void Delete(Comment comment);

    }
}
