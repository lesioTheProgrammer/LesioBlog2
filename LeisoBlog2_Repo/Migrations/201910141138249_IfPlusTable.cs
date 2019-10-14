namespace LesioBlog2_Repo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IfPlusTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.IfPlusowalWpis",
                c => new
                    {
                        UserID = c.Int(nullable: false),
                        WpisID = c.Int(nullable: false),
                        IfPlusWpis = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserID, t.WpisID })
                .ForeignKey("dbo.User", t => t.UserID)
                .ForeignKey("dbo.Wpis", t => t.WpisID)
                .Index(t => t.UserID)
                .Index(t => t.WpisID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.IfPlusowalWpis", "WpisID", "dbo.Wpis");
            DropForeignKey("dbo.IfPlusowalWpis", "UserID", "dbo.User");
            DropIndex("dbo.IfPlusowalWpis", new[] { "WpisID" });
            DropIndex("dbo.IfPlusowalWpis", new[] { "UserID" });
            DropTable("dbo.IfPlusowalWpis");
        }
    }
}
