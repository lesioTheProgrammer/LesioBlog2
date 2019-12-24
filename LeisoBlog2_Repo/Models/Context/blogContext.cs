using LesioBlog2_Repo.Abstract;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace LesioBlog2_Repo.Models.Context
{
    public class BlogContext : DbContext, IBlogContext
    {
        public BlogContext() : base("LesioBlo")
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
        public DbSet<Post> Post { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CommentTag> CommentTags { get; set; }
        public DbSet<PostTag> PostTags { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<IsPostUpvd> IsPostUpvd { get; set; }
        public DbSet<IsCommUpvoted> IsCommUpvoted { get; set; }

        //new
        public DbSet<Role> Roles { get; set; }

        public DbSet<Code> Code { get; set; }


        public DbSet<Messages> Messages { get; set; }
        public DbSet<UserMess> UserMess { get; set; }



        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            //fluent API
            //one to many
            //user has many wpis i many comments
            modelBuilder.Entity<Post>().HasRequired(x => x.User).WithMany(x => x.Post).HasForeignKey(x => x.User_Id).WillCascadeOnDelete(true);
            modelBuilder.Entity<Comment>().HasRequired(x => x.User).WithMany(x => x.Comments).HasForeignKey(x => x.User_Id).WillCascadeOnDelete(true);



            modelBuilder.Entity<User>().HasRequired(x => x.Gender).WithMany(x => x.User).HasForeignKey(x => x.Gender_Id); //role same as gender
            //role
            modelBuilder.Entity<User>().HasRequired(x => x.Role).WithMany(x => x.User).HasForeignKey(x => x.Role_Id);



            //many to many
            modelBuilder.Entity<PostTag>().HasKey(key => new { key.Tag_Id, key.Post_Id });
            modelBuilder.Entity<CommentTag>().HasKey(key => new { key.Tag_Id, key.Comment_Id });
            //mess
            modelBuilder.Entity<UserMess>().HasKey(key => new { key.Message_Id, key.User_Id });


            modelBuilder.Entity<IsPostUpvd>().HasKey(key => new { key.User_Id, key.Post_Id });
            modelBuilder.Entity<IsCommUpvoted>().HasKey(key => new { key.User_Id, key.Comment_Id });

            //one to one or zero
            //code
            modelBuilder.Entity<User>().HasOptional(x => x.Code).WithRequired(u => u.User);
        }
    }
}