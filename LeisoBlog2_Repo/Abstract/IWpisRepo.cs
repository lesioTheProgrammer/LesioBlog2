using LesioBlog2_Repo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeisoBlog2_Repo.Abstract
{
    public interface IWpisRepo : IDisposable
    {
        IQueryable<Wpis> GetWpis();
        List<Wpis> GetWpisByUserNickName(string name);
        Wpis GetWpisById(int? id);
        void SaveChanges();
        void Add(Wpis wpis);
        void Update(Wpis wpis);
        IQueryable<Wpis> GetPages(int? page, int? pagesize);

        DateTime GetWpisWithAddDate(Wpis wpis);




        void Delete(Wpis wpis);



    }
}
