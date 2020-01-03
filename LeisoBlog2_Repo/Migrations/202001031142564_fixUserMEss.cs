namespace LesioBlog2_Repo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixUserMEss : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserMess", "Users_User_Id", "dbo.User");
            DropIndex("dbo.UserMess", new[] { "Users_User_Id" });
            DropColumn("dbo.UserMess", "Users_User_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserMess", "Users_User_Id", c => c.Int());
            CreateIndex("dbo.UserMess", "Users_User_Id");
            AddForeignKey("dbo.UserMess", "Users_User_Id", "dbo.User", "User_Id");
        }
    }
}
