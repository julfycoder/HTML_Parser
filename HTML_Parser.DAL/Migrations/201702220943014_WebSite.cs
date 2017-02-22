namespace HTML_Parser.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WebSite : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WebSites",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        URL = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.WebPages", "WebSiteId", c => c.Int(nullable: false));
            CreateIndex("dbo.WebPages", "WebSiteId");
            AddForeignKey("dbo.WebPages", "WebSiteId", "dbo.WebSites", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WebPages", "WebSiteId", "dbo.WebSites");
            DropIndex("dbo.WebPages", new[] { "WebSiteId" });
            DropColumn("dbo.WebPages", "WebSiteId");
            DropTable("dbo.WebSites");
        }
    }
}
