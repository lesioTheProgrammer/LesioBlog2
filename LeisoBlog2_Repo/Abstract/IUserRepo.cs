using LesioBlog2_Repo.Models;
using System;

namespace LesioBlog2_Repo.Abstract
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
        int? GetIDOfCurrentlyLoggedUser();

        bool CheckIfUserEmailVaild(string email);
        void UpdateOnlyCode(User user);
        User GetUserByID(int id);


    }
}
