namespace LesioBlog2_Repo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserMessFix2 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.UserMess");
            AddPrimaryKey("dbo.UserMess", new[] { "User_Id", "Message_Id" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.UserMess");
            AddPrimaryKey("dbo.UserMess", new[] { "Message_Id", "User_Id" });
        }
    }
}
