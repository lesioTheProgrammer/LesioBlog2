namespace LesioBlog2_Repo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Code : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.User", "Code", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.User", "Code");
        }
    }
}
