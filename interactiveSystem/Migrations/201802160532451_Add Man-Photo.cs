namespace interactiveSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddManPhoto : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Man_Photo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        GalleryType = c.String(),
                        FolderName = c.String(),
                        OrgFolderName = c.String(),
                        FileName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Man_Photo");
        }
    }
}
