using LeisoBlog2_Repo.Models;
using LesioBlog2_Repo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeisoBlog2_Repo.Abstract
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

        Database Database { get; }
        DbEntityEntry Entry(object entity);

        void Dispose();
        int SaveChanges();


    }
}
