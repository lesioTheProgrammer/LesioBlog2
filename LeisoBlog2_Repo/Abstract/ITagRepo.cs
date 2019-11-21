﻿using LesioBlog2_Repo.Models;
using System.Collections.Generic;
using System.Linq;

namespace LesioBlog2_Repo.Abstract
{
    public interface ITagRepo
    {
        IQueryable<Tag> GetTags();

        Tag GetTagByName(string name);

        void Add(Tag tag);
        void SaveChanges();
        string GetTagNamesByTagID(int? id);
        List<Post> getWpisWithSelectedTag(string tagName);
        bool IfWpisOrCommentsHasTag(int id);
        void RemoveTagsIfNotUsed(int id);

        void RemoveWpisTag(int id, int id2);
        void RemoveCommentTag(int id, int id2);
        bool CheckIfCommTagExist(int tagID, int commID);
        bool CheckIfWpisTagExist(int tagID, int wpisID);



    }
}
