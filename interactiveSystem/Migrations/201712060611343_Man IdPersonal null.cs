namespace interactiveSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ManIdPersonalnull : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Men", "IdPersonal");
            AddColumn("dbo.Men", "IdPersonal",
                builder => builder.Int(nullable: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Men", "IdPersonal");
            AddColumn("dbo.Men", "IdPersonal",
                builder => builder.Guid(nullable: true));
        }
    }
}
