using LesioBlog2_Repo.Models;
using System;
using System.Collections.Generic;

namespace LesioBlog2_Repo.Abstract
{
    public interface ICommentRepo : IDisposable
    {
        List<Comment> GetCommentByUserNickName(string name);
        Comment GetCommentById(int? id);
        void SaveChanges();
        void Add(Comment comment);
        DateTime GetCommentWithAddingDate(Comment comment);
        ICollection<Comment> GetCommentByUserID(int? id);
        void Delete(Comment comment);
        int GetIdOfCommentCreator(int? id);
        void UpdateContentAndPlusyAndEditDate(Comment comment);
        IsCommUpvoted GetPlusComment(int? idComment, int? idUser);
        void Add(CommentTag commTag);
        void Add(IsCommUpvoted plusComm);
        void UpdateOnlyVotes(Comment comment);
        void UpdateIfCommState(IsCommUpvoted ifplus);
        List<CommentTag> GetAllCommTagsByCommId(int? id);
    }
}
