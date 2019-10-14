namespace LesioBlog2_Repo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ifPlusowane : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Comment", "IfPlusowane", c => c.Boolean(nullable: false));
            AddColumn("dbo.Wpis", "IfPlusowane", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Wpis", "IfPlusowane");
            DropColumn("dbo.Comment", "IfPlusowane");
        }
    }
}
