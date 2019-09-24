﻿using LeisoBlog2_Repo.Abstract;
using LesioBlog2_Repo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeisoBlog2_Repo.Concrete
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
            var tag = _db.Tags.Include("TagName").SingleOrDefault(x => x.TagName == name);
            return tag;
        }

        public IQueryable<Tag> GetTags()
        {
            var tags = _db.Tags.Include("TagName");
            return tags;
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