namespace HTML_Parser.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NullParentWebPage : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.WebPages", new[] { "ParentPageId" });
            AlterColumn("dbo.WebPages", "ParentPageId", c => c.Int());
            CreateIndex("dbo.WebPages", "ParentPageId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.WebPages", new[] { "ParentPageId" });
            AlterColumn("dbo.WebPages", "ParentPageId", c => c.Int(nullable: false));
            CreateIndex("dbo.WebPages", "ParentPageId");
        }
    }
}
