using LesioBlog2_Repo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeisoBlog2_Repo.Abstract
{
    public interface IUserRepo : IDisposable
    {
        void Add(User user);

        User GetUserByEmail(string email);


        void Delete(User user);

        void SaveChanges();

         User FindUserByID(int? id);
        string GetUserNicknameByEmail(string email);
        User GetUserByNickname(string nickname);

    }
}
