namespace LesioBlog2_Repo.Migrations
{
    using LesioBlog2_Repo.Models;
    using LesioBlog2_Repo.Models;
    using LesioBlog2_Repo.Models.Context;
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BlogContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(BlogContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            SeedGenders(context);
            SeedRoles(context);
         //   SeedUsers(context);
          //  SeedWpis(context);
          //  SeedComments(context);
          //  SeedTags(context);
          //  SeedWpisTag(context);
          //  SeedCommentTag(context);
          //  SeedIfPlus(context);
          //  SeedIfPlusWpis(context);


        }

        private void SeedIfPlusWpis(BlogContext context)
        {

            var ifplus = new IsCommUpvoted
            {
                Comment_Id = context.Comments.FirstOrDefault().Comment_Id,
                User_Id = context.Users.FirstOrDefault().User_Id,
                IsCommentUpvoted = false
            };
            context.Set<IsCommUpvoted>().AddOrUpdate(ifplus);

            context.SaveChanges();
        }





        private void SeedIfPlus(BlogContext context)
        {

            var ifplus = new IsPostUpvd
            {
                Post_Id = 2,
                User_Id = 3,
                IsPostUpvoted = false
            };
            context.Set<IsPostUpvd>().AddOrUpdate(ifplus);

            context.SaveChanges();
        }


        private void SeedComments(BlogContext context)
        {
            //rnd do plusow xd
            var random = new Random();

            for (int i = 1; i < 11; i++)
            {
                var comment = new Comment()
                {
                    Comment_Id = i,
                    User_Id = context.Users.FirstOrDefault().User_Id,
                    Post_Id = context.Post.FirstOrDefault().Post_Id,
                    Content = "Gerara" + i.ToString(),
                    AddingDate = DateTime.Now,
                    Votes = random.Next(0, 100),
                    EditingDate = DateTime.Now.AddDays(1)

                };
                context.Set<Comment>().AddOrUpdate(comment);
            }
            context.SaveChanges();
        }


        private void SeedWpis(BlogContext context)
        {
            var random = new Random();
            for (int i = 1; i < 11; i++)
            {
                var wpis = new Post
                {
                    Post_Id = i,
                    User_Id = context.Users.FirstOrDefault().User_Id,
                    Content = "Mnibu" + i.ToString(),
                    AddingDate = DateTime.Now,
                    Votes = random.Next(0, 100),
                    EditingDate = DateTime.Now.AddDays(3)


                };
                context.Set<Post>().AddOrUpdate(wpis);
            }
            context.SaveChanges();
        }

        private void SeedTags(BlogContext context)
        {
            for (int i = 1; i < 11; i++)
            {
                var tag = new Tag
                {
                    Tag_Id = i,
                    TagName = "ForReal" + i.ToString()
                };
                context.Set<Tag>().AddOrUpdate(tag);
            }
            context.SaveChanges();
        }

        private void SeedGenders(BlogContext context)
        {

            for (int i = 1; i < 3; i++)
            {
                var gender = new Gender
                {
                    Gender_Id = i,
                    GenderName = i < 2 ? "Male" : "Female"
                };
                context.Set<Gender>().AddOrUpdate(gender);


            }
            context.SaveChanges();
        }


        public void SeedRoles(BlogContext context)
        {
            var role = new Role
            {
                Role_Id = 1,
                RoleName = "Admin",
                };
            var role2 = new Role
            {
                Role_Id = 2,
                RoleName = "User",
            }; var role3 = new Role
            {
                Role_Id = 3,
                RoleName = "Moderator",
            };

            context.Set<Role>().AddOrUpdate(role);
            context.Set<Role>().AddOrUpdate(role2);
            context.Set<Role>().AddOrUpdate(role3);
        }



        private void SeedUsers(BlogContext context)
        {
            for (int i = 1; i < 11; i++)
            {
                var user = new User
                {
                    User_Id = i,
                    NickName = "Bobas" + i.ToString(),
                    FullName = "Pan Bobas" + i.ToString(),
                    City = "Breslau" + i.ToString(),
                    Email = "lesio" + i.ToString() + "@gmail.com",
                    Password = "piespies" + i.ToString(),
                    Gender_Id = i < 5 ? 1 : 2,
                };
                context.Set<User>().AddOrUpdate(user);
            }
            context.SaveChanges();
        }


        private void SeedWpisTag(BlogContext context)
        {
            for (int i = 1; i < 11; i++)
            {
                var wptag = new PostTag
                {
                    Post_Id = i,
                    Tag_Id = i
                };
                context.Set<PostTag>().AddOrUpdate(wptag);
            }
            context.SaveChanges();
        }

        private void SeedCommentTag(BlogContext context)
        {
            for (int i = 1; i < 11; i++)
            {
                var comtag = new CommentTag
                {
                    Comment_Id = i,
                    Tag_Id = i
                };
                context.Set<CommentTag>().AddOrUpdate(comtag);
            }
            context.SaveChanges();
        }
    }
}
