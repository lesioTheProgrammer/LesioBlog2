using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace LesioBlog2_Repo.Models
{
    public class BlogContext : DbContext
    {
        public BlogContext() : base("LesioBlog")
        {
            Database.SetInitializer<BlogContext>(null); //remove dafult initializer
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;

        }

        public static BlogContext Create()
        {
            return new BlogContext();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Wpis> Wpis { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<CommentTag> CommentTags { get; set; }

        public DbSet<WpisTag> WpisTags { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();



            //fluent API
            //one to many
            modelBuilder.Entity<Wpis>().HasRequired(x => x.User).WithMany(x => x.Wpis).HasForeignKey(x => x.UserID).WillCascadeOnDelete(true);
            modelBuilder.Entity<Comment>().HasRequired(x => x.User).WithMany(x => x.Comments).HasForeignKey(x => x.UserID).WillCascadeOnDelete(true);
            //many to many
            modelBuilder.Entity<WpisTag>().HasKey(key => new { key.TagID, key.WpisID });
            modelBuilder.Entity<CommentTag>().HasKey(key => new { key.TagID, key.CommentID });


        }


        


    }
}