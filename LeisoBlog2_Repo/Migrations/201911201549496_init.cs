namespace LesioBlog2_Repo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Comment",
                c => new
                    {
                        Comment_Id = c.Int(nullable: false, identity: true),
                        User_Id = c.Int(nullable: false),
                        Post_Id = c.Int(nullable: false),
                        Content = c.String(nullable: false),
                        AddingDate = c.DateTime(nullable: false),
                        Votes = c.Int(nullable: false),
                        EditingDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Comment_Id)
                .ForeignKey("dbo.Post", t => t.Post_Id)
                .ForeignKey("dbo.User", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.User_Id)
                .Index(t => t.Post_Id);
            
            CreateTable(
                "dbo.CommentTag",
                c => new
                    {
                        Tag_Id = c.Int(nullable: false),
                        Comment_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Tag_Id, t.Comment_Id })
                .ForeignKey("dbo.Comment", t => t.Comment_Id)
                .ForeignKey("dbo.Tag", t => t.Tag_Id)
                .Index(t => t.Tag_Id)
                .Index(t => t.Comment_Id);
            
            CreateTable(
                "dbo.Tag",
                c => new
                    {
                        Tag_Id = c.Int(nullable: false, identity: true),
                        TagName = c.String(),
                    })
                .PrimaryKey(t => t.Tag_Id);
            
            CreateTable(
                "dbo.PostTag",
                c => new
                    {
                        Tag_Id = c.Int(nullable: false),
                        Post_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Tag_Id, t.Post_Id })
                .ForeignKey("dbo.Post", t => t.Post_Id)
                .ForeignKey("dbo.Tag", t => t.Tag_Id)
                .Index(t => t.Tag_Id)
                .Index(t => t.Post_Id);
            
            CreateTable(
                "dbo.Post",
                c => new
                    {
                        Post_Id = c.Int(nullable: false, identity: true),
                        User_Id = c.Int(nullable: false),
                        Content = c.String(nullable: false),
                        AddingDate = c.DateTime(nullable: false),
                        Votes = c.Int(nullable: false),
                        EditingDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Post_Id)
                .ForeignKey("dbo.User", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        User_Id = c.Int(nullable: false, identity: true),
                        Gender_Id = c.Int(nullable: false),
                        Email = c.String(nullable: false),
                        Password = c.String(nullable: false, maxLength: 200),
                        PasswordSalt = c.String(),
                        NickName = c.String(nullable: false),
                        FullName = c.String(),
                        City = c.String(),
                        Code = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.User_Id)
                .ForeignKey("dbo.Gender", t => t.Gender_Id)
                .Index(t => t.Gender_Id);
            
            CreateTable(
                "dbo.Gender",
                c => new
                    {
                        Gender_Id = c.Int(nullable: false, identity: true),
                        GenderName = c.String(),
                    })
                .PrimaryKey(t => t.Gender_Id);
            
            CreateTable(
                "dbo.IsPostUpvd",
                c => new
                    {
                        User_Id = c.Int(nullable: false),
                        Post_Id = c.Int(nullable: false),
                        IsPostUpvoted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.User_Id, t.Post_Id })
                .ForeignKey("dbo.Post", t => t.Post_Id)
                .ForeignKey("dbo.User", t => t.User_Id)
                .Index(t => t.User_Id)
                .Index(t => t.Post_Id);
            
            CreateTable(
                "dbo.IsCommUpvoted",
                c => new
                    {
                        User_Id = c.Int(nullable: false),
                        Comment_Id = c.Int(nullable: false),
                        IsCommentUpvoted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.User_Id, t.Comment_Id })
                .ForeignKey("dbo.Comment", t => t.Comment_Id)
                .ForeignKey("dbo.User", t => t.User_Id)
                .Index(t => t.User_Id)
                .Index(t => t.Comment_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.IsCommUpvoted", "User_Id", "dbo.User");
            DropForeignKey("dbo.IsCommUpvoted", "Comment_Id", "dbo.Comment");
            DropForeignKey("dbo.Comment", "User_Id", "dbo.User");
            DropForeignKey("dbo.PostTag", "Tag_Id", "dbo.Tag");
            DropForeignKey("dbo.Post", "User_Id", "dbo.User");
            DropForeignKey("dbo.IsPostUpvd", "User_Id", "dbo.User");
            DropForeignKey("dbo.IsPostUpvd", "Post_Id", "dbo.Post");
            DropForeignKey("dbo.User", "Gender_Id", "dbo.Gender");
            DropForeignKey("dbo.PostTag", "Post_Id", "dbo.Post");
            DropForeignKey("dbo.Comment", "Post_Id", "dbo.Post");
            DropForeignKey("dbo.CommentTag", "Tag_Id", "dbo.Tag");
            DropForeignKey("dbo.CommentTag", "Comment_Id", "dbo.Comment");
            DropIndex("dbo.IsCommUpvoted", new[] { "Comment_Id" });
            DropIndex("dbo.IsCommUpvoted", new[] { "User_Id" });
            DropIndex("dbo.IsPostUpvd", new[] { "Post_Id" });
            DropIndex("dbo.IsPostUpvd", new[] { "User_Id" });
            DropIndex("dbo.User", new[] { "Gender_Id" });
            DropIndex("dbo.Post", new[] { "User_Id" });
            DropIndex("dbo.PostTag", new[] { "Post_Id" });
            DropIndex("dbo.PostTag", new[] { "Tag_Id" });
            DropIndex("dbo.CommentTag", new[] { "Comment_Id" });
            DropIndex("dbo.CommentTag", new[] { "Tag_Id" });
            DropIndex("dbo.Comment", new[] { "Post_Id" });
            DropIndex("dbo.Comment", new[] { "User_Id" });
            DropTable("dbo.IsCommUpvoted");
            DropTable("dbo.IsPostUpvd");
            DropTable("dbo.Gender");
            DropTable("dbo.User");
            DropTable("dbo.Post");
            DropTable("dbo.PostTag");
            DropTable("dbo.Tag");
            DropTable("dbo.CommentTag");
            DropTable("dbo.Comment");
        }
    }
}
