using LesioBlog2_Repo.Models;
using System.Collections.Generic;
using System.Linq;

namespace LeisoBlog2_Repo.Abstract
{
    public interface ITagRepo
    {
        IQueryable<Tag> GetTags();

        Tag GetTagByName(string name);

        void Add(Tag tag);
        void SaveChanges();
        List<Wpis> getWpisWithSelectedTag(string tagName);
    }
}
