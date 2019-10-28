using LesioBlog2_Repo.Abstract;
using LesioBlog2_Repo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace LesioBlog2_Repo.Concrete
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

        public void Add(WpisTag wpisTag)
        {
            _db.WpisTags.Add(wpisTag);
        }


        public void Add(IfPlusowalWpis ifplusowal)
        {
            _db.IfPlusowalWpis.Add(ifplusowal);

        }

        public IfPlusowalWpis GetPlusWpis(int? idWpis, int? idUser)
        {
            var plusedWpis = _db.IfPlusowalWpis
                .Where(x => x.WpisID == idWpis)
                .Where(x => x.UserID == idUser)
                .SingleOrDefault();
            return plusedWpis;
        }


        public void Delete(Wpis wpis)
        {
            //remove comments proper way
            #region
            var commentsChildren = _db.Comments.Where(x => x.WpisID == wpis.WpisID).Select(x=>x);
            
            var commentChildenlist = commentsChildren.ToList(); // to avoid dataReader excep - caused by iterating on IQueryable
            foreach (var item in commentChildenlist)
            {
                //komenty gerara 
                var commTags = _db.CommentTags.Where(x => x.CommentID == item.CommentID).Select(x => x);
                //lista wpistagow'
                //Ilist because this allows to perform Savechanges in Foreach

                IList<CommentTag> listaCommentLoop = _db.CommentTags.Where(x => x.CommentID == item.CommentID)
                    .Select(x => x)
                    .ToList();

                foreach (var item2 in listaCommentLoop)
                {
                    var tagID = item2.TagID;
                    //get list of ID 
                    _db.CommentTags.Remove(item2);
                    //save changes
                    _db.SaveChanges();
                    if (!_db.CommentTags.Any(x => x.TagID == tagID) && !_db.WpisTags.Any(x => x.TagID == tagID))
                    {
                        var tagToRemove = _db.Tags.Where(x => x.TagID == tagID).SingleOrDefault();
                        _db.Tags.Remove(tagToRemove);
                    }
                }
                IList<IfPlusowalComment> listaifPlusComm = _db.IfPlusowalComment
                .Where(x => x.CommentID == item.CommentID)
                .ToList();

                foreach (var item3 in listaifPlusComm)
                {
                    _db.IfPlusowalComment.Remove(item3);
                    _db.SaveChanges();
                }

                _db.Comments.Remove(item);

            }
            #endregion

            //remove tags if are not used anywhere else
            var wpisTags = _db.WpisTags.Where(x => x.WpisID == wpis.WpisID).Select(x => x);
            //lista wpistagow'
            //Ilist because this allows to perform Savechanges in Foreach
            IList<WpisTag> listaforLoop = _db.WpisTags.Where(x => x.WpisID == wpis.WpisID)
                .Select(x => x)
                .ToList();

            foreach (var item in listaforLoop)
            {
                var tagID = item.TagID;
                //get list of ID 
                _db.WpisTags.Remove(item);
                //save changes
                _db.SaveChanges();
                if (!_db.WpisTags.Any(x=>x.TagID == tagID) && !_db.CommentTags.Any(x => x.TagID == tagID))
                {
                    var tagToRemove =_db.Tags.Where(x => x.TagID == tagID).SingleOrDefault();
                    _db.Tags.Remove(tagToRemove);
                }
            }
            IList<IfPlusowalWpis> listaIfPlusWpis = _db.IfPlusowalWpis
              .Where(x => x.WpisID == wpis.WpisID)
              .ToList();

            foreach (var item in listaIfPlusWpis)
            {
                _db.IfPlusowalWpis.Remove(item);
                _db.SaveChanges();
            }


            _db.Wpis.Remove(wpis);
        }

        public void DeleteTagAndWpisTag(int? id)
        {

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
            var listofWpis = _db.Wpis.Include(x => x.Comments).Include(x => x.User).Include(x=>x.Comments.Select(u=>u.User));
            return listofWpis;
        }

        public Wpis GetWpisById(int? id)
        {
            var wpis = _db.Wpis.Include(x=>x.User).Include(x => x.WpisTags).Include(x => x.Comments.Select(c=>c.User)).SingleOrDefault(x => x.WpisID == id);
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
                var wpis = _db.Wpis.Where(x => x.UserID == user.UserID).Include(x=>x.Comments.Select(u=>u.User)).Include(x=>x.User).Select(x=>x).ToList();
                return wpis;
            }
            //else:
            return new List<Wpis>();
        }

        public List<Wpis> GetWpisCointaintnCommWithNickname(string name)
        {
            var user = _db.Users.FirstOrDefault(x => x.NickName.ToLower() == name.ToLower());
            if (user != null)
            {
                var wpis = _db.Wpis
                    .Where (x=>x.Comments.Any(c=>c.UserID== user.UserID))
                    .Include(x => x.Comments.Select(u => u.User))
                    .Include(x => x.User)
                    .ToList();
                return wpis;

            }
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


        public List<WpisTag> GetAllWpisTagsByWpisId(int? id)
        {

            var returning = _db.WpisTags.Where(x => x.WpisID == id).Select(x => x).ToList();
            return returning;
        }
      

        public void SaveChanges()
        {
            _db.SaveChanges();
        }

        public void Update(Wpis wpis)
        {
            _db.Entry(wpis).State = EntityState.Modified;
        }

        public void UpdateContentAndPlusyAndEditDate(Wpis wpis)
        {
            _db.Wpis.Attach(wpis);
            _db.Entry(wpis).Property("Content").IsModified = true;
            _db.Entry(wpis).Property("Plusy").IsModified = true;
            _db.Entry(wpis).Property("EditingDate").IsModified = true;
        }


        public void UpdateOnlyPlusy(Wpis wpis)
        {
            _db.Wpis.Attach(wpis);
            _db.Entry(wpis).Property("Plusy").IsModified = true;
        }

        public void UpdateIfWpisState(IfPlusowalWpis ifplus)
        {
            _db.IfPlusowalWpis.Attach(ifplus);
            _db.Entry(ifplus).Property("IfPlusWpis").IsModified = true;
        }

    }
}