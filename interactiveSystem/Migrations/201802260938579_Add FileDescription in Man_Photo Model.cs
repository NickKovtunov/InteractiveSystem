namespace interactiveSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFileDescriptioninMan_PhotoModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Man_Photo", "FileDescription", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Man_Photo", "FileDescription");
        }
    }
}
