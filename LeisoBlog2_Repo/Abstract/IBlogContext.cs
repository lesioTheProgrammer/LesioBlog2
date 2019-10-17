﻿using LesioBlog2_Repo.Models;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace LesioBlog2_Repo.Abstract
{
    public  interface IBlogContext
    {
         DbSet<User> Users { get; set; }
         DbSet<Tag> Tags { get; set; }
         DbSet<Wpis> Wpis { get; set; }

         DbSet<Comment> Comments { get; set; }

         DbSet<CommentTag> CommentTags { get; set; }

         DbSet<WpisTag> WpisTags { get; set; }

         DbSet<Gender> Genders { get; set; }

        DbSet<IfPlusowalWpis> IfPlusowalWpis { get; set; }
         DbSet<IfPlusowalComment> IfPlusowalComment { get; set; }


        Database Database { get; }
        DbEntityEntry Entry(object entity);

        void Dispose();
        int SaveChanges();


    }
}
