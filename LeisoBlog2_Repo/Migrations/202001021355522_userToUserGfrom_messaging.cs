namespace LesioBlog2_Repo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userToUserGfrom_messaging : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Messages", "UserFrom_Id", c => c.Int(nullable: false));
            AddColumn("dbo.Messages", "UserTo_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.Messages", "UserFrom_Id");
            CreateIndex("dbo.Messages", "UserTo_Id");
            AddForeignKey("dbo.Messages", "UserFrom_Id", "dbo.User", "User_Id");
            AddForeignKey("dbo.Messages", "UserTo_Id", "dbo.User", "User_Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Messages", "UserTo_Id", "dbo.User");
            DropForeignKey("dbo.Messages", "UserFrom_Id", "dbo.User");
            DropIndex("dbo.Messages", new[] { "UserTo_Id" });
            DropIndex("dbo.Messages", new[] { "UserFrom_Id" });
            DropColumn("dbo.Messages", "UserTo_Id");
            DropColumn("dbo.Messages", "UserFrom_Id");
        }
    }
}
