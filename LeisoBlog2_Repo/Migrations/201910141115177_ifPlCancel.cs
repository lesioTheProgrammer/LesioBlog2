namespace LesioBlog2_Repo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ifPlCancel : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Comment", "IfPlusowane");
            DropColumn("dbo.Wpis", "IfPlusowane");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Wpis", "IfPlusowane", c => c.Boolean(nullable: false));
            AddColumn("dbo.Comment", "IfPlusowane", c => c.Boolean(nullable: false));
        }
    }
}
