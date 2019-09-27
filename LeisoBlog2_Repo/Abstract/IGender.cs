using LeisoBlog2_Repo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeisoBlog2_Repo.Abstract
{
      public  interface IGender
    {
        IQueryable<Gender> GetGenders();

        void Add(Gender gender);

        void SaveChanges();
        Gender GetGenderByID(int id);


    }
}




