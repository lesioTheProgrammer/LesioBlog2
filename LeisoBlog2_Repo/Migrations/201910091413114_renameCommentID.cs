namespace LesioBlog2_Repo.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class renameCommentID : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CommentTag", "CommentID", "dbo.Comment");
            DropPrimaryKey("dbo.Comment");
            DropColumn("dbo.Comment", "ID");
            AddColumn("dbo.Comment", "CommentID", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Comment", "CommentID");
            AddForeignKey("dbo.CommentTag", "CommentID", "dbo.Comment", "CommentID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Comment", "ID", c => c.Int(nullable: false, identity: true));
            DropForeignKey("dbo.CommentTag", "CommentID", "dbo.Comment");
            DropPrimaryKey("dbo.Comment");
            DropColumn("dbo.Comment", "CommentID");
            AddPrimaryKey("dbo.Comment", "ID");
            AddForeignKey("dbo.CommentTag", "CommentID", "dbo.Comment", "ID");
        }
    }
}
