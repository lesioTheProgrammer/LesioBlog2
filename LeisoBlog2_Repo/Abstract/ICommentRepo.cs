using LesioBlog2_Repo.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LesioBlog2_Repo.Abstract
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

        Comment FindCommentByID(int? id);
        void Delete(Comment comment);
        int GetIdOfCommentCreator(int? id);
        void UpdateContentAndPlusyAndEditDate(Comment comment);


        void Add(CommentTag commTag);


    }
}
