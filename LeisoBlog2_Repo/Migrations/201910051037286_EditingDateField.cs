namespace LesioBlog2_Repo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EditingDateField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Comment", "EditingDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Wpis", "EditingDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Wpis", "EditingDate");
            DropColumn("dbo.Comment", "EditingDate");
        }
    }
}
