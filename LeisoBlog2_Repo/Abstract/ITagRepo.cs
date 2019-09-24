using LesioBlog2_Repo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeisoBlog2_Repo.Abstract
{
    public interface ITagRepo
    {
        IQueryable<Tag> GetTags();

        Tag GetTagByName(string name);

        void Add(Tag tag);
        void SaveChanges();


    }
}
