namespace LesioBlog2_Repo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class editingDateToNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Comment", "EditingDate", c => c.DateTime());
            AlterColumn("dbo.Wpis", "EditingDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Wpis", "EditingDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Comment", "EditingDate", c => c.DateTime(nullable: false));
        }
    }
}
