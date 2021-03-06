﻿using LesioBlog2_Repo.Abstract;
using LesioBlog2_Repo.Models;
using System.Data.Entity;
using System.Linq;

namespace LesioBlog2_Repo.Concrete
{
    public class UserRepo : IUserRepo
    {

        private readonly IBlogContext _db;

        public UserRepo(IBlogContext db)
        {
            this._db = db;
        }

        public void Add(User user)
        {
            _db.Users.Add(user);
        }

        public void Delete(User user)
        {
            _db.Users.Remove(user);
        }

        public User GetUserByEmail(string email)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
           
            return user;
        }

        public User FindUserByID(int? id)
        {
            var user = _db.Users.FirstOrDefault(u => u.User_Id == id);
            return user;
        }


        public string GetUserNicknameByEmail(string email)
        {
            string nickname = "";
            //i got user by email--get userName
            var user = _db.Users.FirstOrDefault(x => x.Email.ToLower() == email.ToLower());
            if (user != null)
            {
              nickname = user.NickName;
              return nickname.ToLower();
            }
            else
            {
              return nickname.ToLower();
            }
        }

        public int? GetIDOfCurrentlyLoggedUser()
        {
            var FakeUser = System.Web.HttpContext.Current.User;
            string userName = FakeUser.Identity.Name;
            if (string.IsNullOrEmpty(userName))
            {
                return null;
            }
            var currentlyLoggedUserId = _db.Users.FirstOrDefault(x=>x.NickName == userName).User_Id;
            return currentlyLoggedUserId;

        }

        public User GetUserByNickname(string nickname)
        {
            var user = _db.Users.FirstOrDefault(x => x.NickName.ToLower() == nickname.ToLower());
            return user;
        }

        public User GetLoggedUser()
        {
            var user =_db.Users.FirstOrDefault();
            return user;
        }
        public void SaveChanges()
        {
            _db.SaveChanges();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _db.Dispose();  
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~UserRepo()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion


        public bool CheckIfUserEmailVaild(string email)
        {
            bool output = false;
            if (_db.Users.Any(x=>x.Email == email))
            {
                output = true;
            }
            return output;
        }

        public User GetUserByID(int id)
        {
             var user =  _db.Users.FirstOrDefault(x => x.User_Id == id);
            return user;
        }

        public User GetUserByIDAndCode(int id)
        {
            var user = _db.Users.Include(z => z.Code).FirstOrDefault(x => x.User_Id == id);
            return user;
        }

    }
}