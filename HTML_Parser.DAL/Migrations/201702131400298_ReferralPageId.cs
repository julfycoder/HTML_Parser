namespace HTML_Parser.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReferralPageId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WebPages", "ReferralPageId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.WebPages", "ReferralPageId");
        }
    }
}
