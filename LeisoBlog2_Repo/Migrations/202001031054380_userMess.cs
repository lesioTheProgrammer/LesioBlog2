namespace LesioBlog2_Repo.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class userMess : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Messages", "UserFrom_Id", "dbo.User");
            DropForeignKey("dbo.Messages", "UserTo_Id", "dbo.User");
            DropForeignKey("dbo.UserMess", "User_Id", "dbo.User");
            DropIndex("dbo.Messages", new[] { "UserFrom_Id" });
            DropIndex("dbo.Messages", new[] { "UserTo_Id" });
            DropIndex("dbo.UserMess", new[] { "User_Id" });
            AddColumn("dbo.UserMess", "UserFrom_Id", c => c.Int(nullable: false));
            AddColumn("dbo.UserMess", "UserTo_Id", c => c.Int(nullable: false));
            AddColumn("dbo.UserMess", "Users_User_Id", c => c.Int());
            CreateIndex("dbo.UserMess", "UserFrom_Id");
            CreateIndex("dbo.UserMess", "UserTo_Id");
            CreateIndex("dbo.UserMess", "Users_User_Id");
            AddForeignKey("dbo.UserMess", "Users_User_Id", "dbo.User", "User_Id");
            DropColumn("dbo.Messages", "UserFrom_Id");
            DropColumn("dbo.Messages", "UserTo_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Messages", "UserTo_Id", c => c.Int(nullable: false));
            AddColumn("dbo.Messages", "UserFrom_Id", c => c.Int(nullable: false));
            DropForeignKey("dbo.UserMess", "Users_User_Id", "dbo.User");
            DropIndex("dbo.UserMess", new[] { "Users_User_Id" });
            DropIndex("dbo.UserMess", new[] { "UserTo_Id" });
            DropIndex("dbo.UserMess", new[] { "UserFrom_Id" });
            DropColumn("dbo.UserMess", "Users_User_Id");
            DropColumn("dbo.UserMess", "UserTo_Id");
            DropColumn("dbo.UserMess", "UserFrom_Id");
            CreateIndex("dbo.UserMess", "User_Id");
            CreateIndex("dbo.Messages", "UserTo_Id");
            CreateIndex("dbo.Messages", "UserFrom_Id");
            AddForeignKey("dbo.UserMess", "User_Id", "dbo.User", "User_Id");
        }
    }
}
