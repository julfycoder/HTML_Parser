namespace HTML_Parser.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CssFiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        WebPageId = c.Int(nullable: false),
                        URL = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WebPages", t => t.WebPageId, cascadeDelete: true)
                .Index(t => t.WebPageId);
            
            CreateTable(
                "dbo.WebPages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TimeToLoad = c.Int(nullable: false),
                        HTML_Size = c.Int(nullable: false),
                        ParentPageId = c.Int(nullable: false),
                        URL = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WebPages", t => t.ParentPageId)
                .Index(t => t.ParentPageId);
            
            CreateTable(
                "dbo.ImageFiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        WebPageId = c.Int(nullable: false),
                        URL = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WebPages", t => t.WebPageId, cascadeDelete: true)
                .Index(t => t.WebPageId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ImageFiles", "WebPageId", "dbo.WebPages");
            DropForeignKey("dbo.CssFiles", "WebPageId", "dbo.WebPages");
            DropForeignKey("dbo.WebPages", "ParentPageId", "dbo.WebPages");
            DropIndex("dbo.ImageFiles", new[] { "WebPageId" });
            DropIndex("dbo.WebPages", new[] { "ParentPageId" });
            DropIndex("dbo.CssFiles", new[] { "WebPageId" });
            DropTable("dbo.ImageFiles");
            DropTable("dbo.WebPages");
            DropTable("dbo.CssFiles");
        }
    }
}
