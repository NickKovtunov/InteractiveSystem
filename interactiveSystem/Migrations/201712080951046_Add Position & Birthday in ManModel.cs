namespace interactiveSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPositionBirthdayinManModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Men", "Position", c => c.String());
            AddColumn("dbo.Men", "Birthday", c => c.DateTime(nullable: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Men", "Birthday");
            DropColumn("dbo.Men", "Position");
        }
    }
}
