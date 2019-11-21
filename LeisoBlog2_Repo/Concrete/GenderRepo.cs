using LesioBlog2_Repo.Abstract;
using LesioBlog2_Repo.Models;
using System.Linq;

namespace LesioBlog2_Repo.Concrete
{
    public class GenderRepo : IGender
    {

        private readonly IBlogContext _db;

        public GenderRepo(IBlogContext db)
        {
            this._db = db;
        }
        public void Add(Gender gender)
        {
            _db.Genders.Add(gender);

        }
        public IQueryable<Gender> GetGenders()
        {
            var genders = _db.Genders;
            return genders;
        }


        public Gender GetGenderByID(int id)
        {
            var gender = _db.Genders.FirstOrDefault(x => x.Gender_Id == id);
            if (gender == null)
            {
                return new Gender();
            }
            return gender;
        }

        public void SaveChanges()
        {
            _db.SaveChanges();
        }
    }
}