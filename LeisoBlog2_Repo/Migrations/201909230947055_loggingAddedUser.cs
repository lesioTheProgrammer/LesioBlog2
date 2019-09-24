namespace LeisoBlog2_Repo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class loggingAddedUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.User", "Email", c => c.String(nullable: false));
            AddColumn("dbo.User", "Password", c => c.String(nullable: false, maxLength: 200));
            AddColumn("dbo.User", "PasswordSalt", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.User", "PasswordSalt");
            DropColumn("dbo.User", "Password");
            DropColumn("dbo.User", "Email");
        }
    }
}
