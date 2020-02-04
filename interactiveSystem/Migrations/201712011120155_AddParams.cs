namespace interactiveSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddParams : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Enterprises", "IdOrg", c => c.Int(nullable: true));
            AddColumn("dbo.Rewards", "IdRew", c => c.Int(nullable: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Rewards", "IdRew");
            DropColumn("dbo.Enterprises", "IdOrg");
        }
    }
}
