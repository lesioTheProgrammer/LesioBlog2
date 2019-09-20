namespace LeisoBlog2_Repo.Migrations
{
    using LesioBlog2_Repo.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<LesioBlog2_Repo.Models.BlogContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(LesioBlog2_Repo.Models.BlogContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            SeedUsers(context);
            SeedWpis(context);
            SeedComments(context);
            SeedTags(context);
            SeedWpisTag(context);
            SeedCommentTag(context);
        }

        private void SeedComments(BlogContext context)
        {
            //rnd do plusow xd
            var random = new Random();

            for (int i = 1; i < 11; i++)
            {
                var comment = new Comment()
                {
                    ID = i,
                    UserID = context.Users.FirstOrDefault().UserID,
                    WpisID = context.Wpis.FirstOrDefault().WpisID,
                    Content = "Gerara" + i.ToString(),
                    AddingDate = DateTime.Now,
                    Plusy = random.Next(0,100)
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
                var wpis = new Wpis
                {
                    WpisID = i,
                    UserID = context.Users.FirstOrDefault().UserID,
                    Content = "Mnibu" + i.ToString(),
                    AddingDate = DateTime.Now,
                    Plusy = random.Next(0, 100)
                };
                context.Set<Wpis>().AddOrUpdate(wpis);
            }
            context.SaveChanges();
        }

        private void SeedTags(BlogContext context)
        {
            for (int i = 1; i < 11; i++)
            {
                var tag = new Tag
                {
                    TagID = i,
                    TagName = "ForReal" + i.ToString()
                };
                context.Set<Tag>().AddOrUpdate(tag);
            }
            context.SaveChanges();
        }

        private void SeedUsers(BlogContext context)
        {
            for (int i = 1; i < 11; i++)
            {
                var user = new User
                {
                    UserID = i,
                    NickName = "Bobas" + i.ToString(),
                    FullName = "Pan Bobas" + i.ToString(),
                    City = "Breslau" + i.ToString(),
                    Gender = i < 5 ? "Male" : "Female"

                };
                context.Set<User>().AddOrUpdate(user);
            }
            context.SaveChanges();
        }


        private void SeedWpisTag(BlogContext context)
        {
            for (int i = 1; i < 11; i++)
            {
                var wptag = new WpisTag
                {
                    WpisID = i,
                    TagID = i
                };
                context.Set<WpisTag>().AddOrUpdate(wptag);
            }
            context.SaveChanges();
        }

        private void SeedCommentTag(BlogContext context)
        {
            for (int i = 1; i < 11; i++)
            {
                var comtag = new CommentTag
                {
                    CommentID = i,
                    TagID = i
                };
                context.Set<CommentTag>().AddOrUpdate(comtag);
            }
            context.SaveChanges();
        }
    }
}
