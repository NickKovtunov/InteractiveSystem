namespace interactiveSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newsModelNewchanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.NewsGalleries", "Preview", c => c.String());
            AddColumn("dbo.NewsGalleries", "PublishingDate", c => c.String());
            AddColumn("dbo.NewsGalleries", "ImagePreview", c => c.String());
            AddColumn("dbo.NewsGalleries", "Title", c => c.String());
            AddColumn("dbo.NewsGalleries", "Text", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.NewsGalleries", "Text");
            DropColumn("dbo.NewsGalleries", "Title");
            DropColumn("dbo.NewsGalleries", "ImagePreview");
            DropColumn("dbo.NewsGalleries", "PublishingDate");
            DropColumn("dbo.NewsGalleries", "Preview");
        }
    }
}
