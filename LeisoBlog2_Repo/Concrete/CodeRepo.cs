using LesioBlog2_Repo.Abstract;
using LesioBlog2_Repo.Models;
using System;
using System.Linq;

namespace LesioBlog2_Repo.Concrete
{
    public class CodeRepo : ICodeRepo
    {
        private readonly IBlogContext _db;

        public CodeRepo(IBlogContext db)
        {
            this._db = db;
        }

        public int AddCode(int userID)
        {
            var rnd = new Random();
            var code = new Code { };

            //if code exist only update codeValue
            //else insert all

            var codeCheck = _db.Code.FirstOrDefault(x => x.User_Id == userID);

            if (codeCheck == null)
            {
                code.CodeValue = rnd.Next(10000, int.MaxValue);
                code.User_Id = userID;
                _db.Code.Add(code);
                _db.SaveChanges();
                return code.CodeValue;
            }
            else
            {
                //update only code
                codeCheck.CodeValue = rnd.Next(10000, int.MaxValue);
                _db.SaveChanges();
                return codeCheck.CodeValue;
            }
            
        }


        public int? GetCodeValue(int id)
        {
            int? codeVal = _db.Code.FirstOrDefault(x => x.User_Id == id).CodeValue;
            if (codeVal != null)
            {
                return codeVal;
            }
            return 0;
        }

        public void SaveChanges()
        {
            throw new NotImplementedException();
        }
    }
}