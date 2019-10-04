using LeisoBlog2_Repo.Abstract;
using LesioBlog2_Repo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace LeisoBlog2_Repo.Concrete
{
    public class WpisRepo : IWpisRepo, IDisposable
    {

        private readonly IBlogContext _db;

        public WpisRepo(IBlogContext db)
        {
            this._db = db;
        }

        public object Database => _db.Database;


        public void Add(Wpis wpis)
        {
            _db.Wpis.Add(wpis);
        }

        public void Delete(Wpis wpis)
        {
            _db.Wpis.Remove(wpis);
        }
        //disposing- garb collecting
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


    



        public IQueryable<Wpis> GetPages(int? page, int? pagesize)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Wpis> GetWpis()
        {
            var listofWpis = _db.Wpis.Include("User").Include(x => x.Comments);
            return listofWpis;
        }

        public Wpis GetWpisById(int? id)
        {
            var wpis = _db.Wpis.Include("User").Include(x => x.Comments).SingleOrDefault(x => x.WpisID == id);
            return wpis;
        }

        public ICollection<Wpis> GetWpisByUserID(int? id)
        {
            //var wpis = _db.Wpis.Where(x => x.UserID == id);

            var wpis = _db.Wpis.Where(x => x.UserID == id).Select(x=>x);
            var list = wpis.ToList();
            return list;
        }

        public List<Wpis> GetWpisByUserNickName(string name)
        {
            var user = _db.Users.FirstOrDefault(x => x.NickName.ToLower() == name.ToLower());
            if (user != null)
            {
                var wpis = _db.Wpis.Where(x => x.UserID == user.UserID).Include(x=>x.Comments).Select(x=>x).ToList();
                return wpis;
            }
            //else:
            return new List<Wpis>();
        }

        public int GetIdOfWpisCreator(int? id)
        {
            int wpisUserId = _db.Wpis.SingleOrDefault(x => x.WpisID == id).UserID;
            return wpisUserId;
        }


        public DateTime GetWpisWithAddDate(Wpis wpis)
        {
            DateTime data;
            //najpierw wpis potem wpisadddate?
            int id = wpis.WpisID;
            //asnotracking zeby zniknelo mi z _db powiazanie z wpisem bo tak bym mial 2 wpisy i jeden bym zmienil
            data = _db.Wpis
                .AsNoTracking()
                .SingleOrDefault(x=>x.WpisID == id).AddingDate;

            

            if (data == null)
            {
                data = DateTime.Now;
            }
            return data;
        }
      

        public void SaveChanges()
        {
            _db.SaveChanges();
        }

        public void Update(Wpis wpis)
        {
            _db.Entry(wpis).State = EntityState.Modified;
        }
    }
}