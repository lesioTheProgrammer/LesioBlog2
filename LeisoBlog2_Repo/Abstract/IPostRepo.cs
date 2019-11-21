using LesioBlog2_Repo.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LesioBlog2_Repo.Abstract
{
    public interface IPostRepo : IDisposable
    {
        IQueryable<Post> GetPost();
        List<Post> GetPostByUsrNickName(string name);
        List<Post> GetPostCointaininCommWithNickname(string name);
        Post GetPostByID(int? id);
        void SaveChanges();
        void Add(Post wpis);

        DateTime GetPostWithAddingDate(Post wpis);

        ICollection<Post> GetPostByUserID(int? id);

        void Delete(Post wpis);
        int GetIdOpfPostCreator(int? id);

        void UpdateContentUpvotesAddDate(Post wpis);
        void Add(PostTag wpisTag);

        void UpdateOnlyVotes(Post wpis);


        List<PostTag> GetAllPostTagsByPostID(int? id);
        void Add(IsPostUpvd ifplusowal);
        IsPostUpvd GetUpvPost(int? idWpis, int? idUser);


        void UpdateIsVotedState(IsPostUpvd ifplus);
    }
}
