using LesioBlog2_Repo.Models;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace LesioBlog2_Repo.Abstract
{
    public  interface IBlogContext
    {
         DbSet<User> Users { get; set; }
         DbSet<Tag> Tags { get; set; }
         DbSet<Post> Post { get; set; }

         DbSet<Comment> Comments { get; set; }

         DbSet<CommentTag> CommentTags { get; set; }

         DbSet<PostTag> PostTags { get; set; }

         DbSet<Gender> Genders { get; set; }

        DbSet<IsPostUpvd> IsPostUpvd { get; set; }
         DbSet<IsCommUpvoted> IsCommUpvoted { get; set; }


        Database Database { get; }
        DbEntityEntry Entry(object entity);

        void Dispose();
        int SaveChanges();


    }
}
