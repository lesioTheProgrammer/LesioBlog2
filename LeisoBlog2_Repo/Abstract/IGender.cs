using LesioBlog2_Repo.Models;
using System.Linq;

namespace LesioBlog2_Repo.Abstract
{
    public  interface IGender
    {
        IQueryable<Gender> GetGenders();
        void Add(Gender gender);
        void SaveChanges();
        Gender GetGenderByID(int id);
    }
}
