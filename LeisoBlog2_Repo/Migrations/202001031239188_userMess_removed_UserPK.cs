namespace LesioBlog2_Repo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userMess_removed_UserPK : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserMess", "Message_Id", "dbo.Messages");
            DropIndex("dbo.UserMess", new[] { "Message_Id" });
            DropPrimaryKey("dbo.UserMess");
            AddColumn("dbo.UserMess", "Messages_Message_Id", c => c.Int());
            AlterColumn("dbo.UserMess", "Message_Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.UserMess", "Message_Id");
            CreateIndex("dbo.UserMess", "Messages_Message_Id");
            AddForeignKey("dbo.UserMess", "Messages_Message_Id", "dbo.Messages", "Message_Id");
            DropColumn("dbo.UserMess", "User_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserMess", "User_Id", c => c.Int(nullable: false));
            DropForeignKey("dbo.UserMess", "Messages_Message_Id", "dbo.Messages");
            DropIndex("dbo.UserMess", new[] { "Messages_Message_Id" });
            DropPrimaryKey("dbo.UserMess");
            AlterColumn("dbo.UserMess", "Message_Id", c => c.Int(nullable: false));
            DropColumn("dbo.UserMess", "Messages_Message_Id");
            AddPrimaryKey("dbo.UserMess", new[] { "User_Id", "Message_Id" });
            CreateIndex("dbo.UserMess", "Message_Id");
            AddForeignKey("dbo.UserMess", "Message_Id", "dbo.Messages", "Message_Id");
        }
    }
}
