namespace LeisoBlog2_Repo.Migrations
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
                        ID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                        WpisID = c.Int(nullable: false),
                        Content = c.String(),
                        AddingDate = c.DateTime(nullable: false),
                        Plusy = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Wpis", t => t.WpisID)
                .ForeignKey("dbo.User", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID)
                .Index(t => t.WpisID);
            
            CreateTable(
                "dbo.CommentTag",
                c => new
                    {
                        TagID = c.Int(nullable: false),
                        CommentID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TagID, t.CommentID })
                .ForeignKey("dbo.Comment", t => t.CommentID)
                .ForeignKey("dbo.Tag", t => t.TagID)
                .Index(t => t.TagID)
                .Index(t => t.CommentID);
            
            CreateTable(
                "dbo.Tag",
                c => new
                    {
                        TagID = c.Int(nullable: false, identity: true),
                        TagName = c.String(),
                    })
                .PrimaryKey(t => t.TagID);
            
            CreateTable(
                "dbo.WpisTag",
                c => new
                    {
                        TagID = c.Int(nullable: false),
                        WpisID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TagID, t.WpisID })
                .ForeignKey("dbo.Tag", t => t.TagID)
                .ForeignKey("dbo.Wpis", t => t.WpisID)
                .Index(t => t.TagID)
                .Index(t => t.WpisID);
            
            CreateTable(
                "dbo.Wpis",
                c => new
                    {
                        WpisID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                        Content = c.String(),
                        AddingDate = c.DateTime(nullable: false),
                        Plusy = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.WpisID)
                .ForeignKey("dbo.User", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        UserID = c.Int(nullable: false, identity: true),
                        NickName = c.String(),
                        FullName = c.String(),
                        City = c.String(),
                        Gender = c.String(),
                    })
                .PrimaryKey(t => t.UserID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Comment", "UserID", "dbo.User");
            DropForeignKey("dbo.WpisTag", "WpisID", "dbo.Wpis");
            DropForeignKey("dbo.Wpis", "UserID", "dbo.User");
            DropForeignKey("dbo.Comment", "WpisID", "dbo.Wpis");
            DropForeignKey("dbo.WpisTag", "TagID", "dbo.Tag");
            DropForeignKey("dbo.CommentTag", "TagID", "dbo.Tag");
            DropForeignKey("dbo.CommentTag", "CommentID", "dbo.Comment");
            DropIndex("dbo.Wpis", new[] { "UserID" });
            DropIndex("dbo.WpisTag", new[] { "WpisID" });
            DropIndex("dbo.WpisTag", new[] { "TagID" });
            DropIndex("dbo.CommentTag", new[] { "CommentID" });
            DropIndex("dbo.CommentTag", new[] { "TagID" });
            DropIndex("dbo.Comment", new[] { "WpisID" });
            DropIndex("dbo.Comment", new[] { "UserID" });
            DropTable("dbo.User");
            DropTable("dbo.Wpis");
            DropTable("dbo.WpisTag");
            DropTable("dbo.Tag");
            DropTable("dbo.CommentTag");
            DropTable("dbo.Comment");
        }
    }
}
