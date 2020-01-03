namespace LesioBlog2_Repo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class brandNewMessages : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserMess", "Messages_Message_Id", "dbo.Messages");
            DropForeignKey("dbo.UserMess", "UserFrom_Id", "dbo.User");
            DropForeignKey("dbo.UserMess", "UserTo_Id", "dbo.User");
            DropIndex("dbo.UserMess", new[] { "UserFrom_Id" });
            DropIndex("dbo.UserMess", new[] { "UserTo_Id" });
            DropIndex("dbo.UserMess", new[] { "Messages_Message_Id" });
            AddColumn("dbo.Messages", "UserFrom_Id", c => c.Int(nullable: false));
            AddColumn("dbo.Messages", "UserTo_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.Messages", "UserFrom_Id");
            CreateIndex("dbo.Messages", "UserTo_Id");
            AddForeignKey("dbo.Messages", "UserFrom_Id", "dbo.User", "User_Id");
            AddForeignKey("dbo.Messages", "UserTo_Id", "dbo.User", "User_Id");
            DropTable("dbo.UserMess");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.UserMess",
                c => new
                    {
                        Message_Id = c.Int(nullable: false, identity: true),
                        UserFrom_Id = c.Int(nullable: false),
                        UserTo_Id = c.Int(nullable: false),
                        Messages_Message_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Message_Id);
            
            DropForeignKey("dbo.Messages", "UserTo_Id", "dbo.User");
            DropForeignKey("dbo.Messages", "UserFrom_Id", "dbo.User");
            DropIndex("dbo.Messages", new[] { "UserTo_Id" });
            DropIndex("dbo.Messages", new[] { "UserFrom_Id" });
            DropColumn("dbo.Messages", "UserTo_Id");
            DropColumn("dbo.Messages", "UserFrom_Id");
            CreateIndex("dbo.UserMess", "Messages_Message_Id");
            CreateIndex("dbo.UserMess", "UserTo_Id");
            CreateIndex("dbo.UserMess", "UserFrom_Id");
            AddForeignKey("dbo.UserMess", "UserTo_Id", "dbo.User", "User_Id");
            AddForeignKey("dbo.UserMess", "UserFrom_Id", "dbo.User", "User_Id");
            AddForeignKey("dbo.UserMess", "Messages_Message_Id", "dbo.Messages", "Message_Id");
        }
    }
}
