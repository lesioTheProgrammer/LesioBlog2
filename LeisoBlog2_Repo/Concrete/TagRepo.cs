using LesioBlog2_Repo.Abstract;
using LesioBlog2_Repo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace LesioBlog2_Repo.Concrete
{
    public class TagRepo : ITagRepo, IDisposable
    {

        private readonly IBlogContext _db;

        public TagRepo(IBlogContext db)
        {
            this._db = db;
        }

        public void Add(Tag tag)
        {
            _db.Tags.Add(tag);
        }

        public Tag GetTagByName(string name)
        {
            var tag = _db.Tags.Include("PostTag").FirstOrDefault(x => x.TagName == name);
            return tag;
        }

        public IQueryable<Tag> GetTags()
        {
            var tags = _db.Tags.Include("PostTag");
            return tags;
        }

        public bool CheckIfCommTagExist(int tagID, int commID)
        {
            bool exist = false;
            var commTag = _db.CommentTags.FirstOrDefault(x => x.Tag_Id == tagID && x.Comment_Id == commID);
            if (commTag != null)
            {
                exist = true;
            }
            return exist;
        }

        public bool CheckIfWpisTagExist(int tagID, int wpisID)
        {
            bool exist = false;
            var commTag = _db.PostTags.FirstOrDefault(x => x.Tag_Id == tagID && x.Post_Id == wpisID);
            if (commTag != null)
            {
                exist = true;
            }
            return exist;
        }

        public string GetTagNamesByTagID(int? id)
        { 
            var returnList = _db.Tags.FirstOrDefault(x => x.Tag_Id == id).TagName;
            return returnList;
        }

        public void RemoveTagsIfNotUsed(int id)
        {
            var tagToRemove = _db.Tags.FirstOrDefault(x => x.Tag_Id == id);
            _db.Tags.Remove(tagToRemove);
        }

        public bool IfWpisOrCommentsHasTag(int id)
        {
            bool hastag = false;
            if (!_db.PostTags.Any(x => x.Tag_Id == id) && !_db.CommentTags.Any(x => x.Tag_Id == id))
            {
                hastag = true;
            }
            return hastag;
        }

        public void RemoveWpisTag(int id, int id2)
        {
            var listaforLoop = _db.PostTags.FirstOrDefault(x => x.Post_Id == id2 && x.Tag_Id == id);
            _db.PostTags.Remove(listaforLoop);
            //save changes
            _db.SaveChanges();
        }

        public void RemoveCommentTag(int id, int id2)
        {
            var listaforLoop = _db.CommentTags.FirstOrDefault(x => x.Comment_Id == id2 && x.Tag_Id == id);
            _db.CommentTags.Remove(listaforLoop);
            //save changes
            _db.SaveChanges();
        }

        public List<Post> getWpisWithSelectedTag(string tagName)
        {
            int tagIdByTagName = _db.Tags
               .FirstOrDefault(x => x.TagName == tagName).Tag_Id;
            var listOfWpisIncludingTags = _db.PostTags
                .Where(x => x.Tag_Id == tagIdByTagName)
                .Select(x => x.Post)
                .Include(x => x.Comments.Select(u => u.User))
                .Include(x => x.User)
                .ToList();
            return listOfWpisIncludingTags;
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
        // ~TagRepo()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}