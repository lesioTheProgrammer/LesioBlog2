using LeisoBlog2_Repo.Abstract;
using LesioBlog2_Repo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeisoBlog2_Repo.Concrete
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
            var user = _db.Users.SingleOrDefault(u => u.Email.ToLower() == email.ToLower());
            return user;
        }

        public User FindUserByID(int? id)
        {
            var user = _db.Users.SingleOrDefault(u => u.UserID == id);
            return user;

        }

        public string GetUserNicknameByEmail(string email)
        {
            string nickname = "THIS USER HAS NO NICKNAME WHATS WRONG HELP";
            //mam usera po emailu tera wygrzebac jego nickname
            var user = _db.Users.SingleOrDefault(x => x.Email.ToLower() == email.ToLower());

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

        public User GetUserByNickname(string nickname)
        {
            var user = _db.Users.SingleOrDefault(x => x.NickName.ToLower() == nickname.ToLower());
            return user;
        }

        public User GetLoggedUser()
        {
            var user =_db.Users.SingleOrDefault();
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




    }
}