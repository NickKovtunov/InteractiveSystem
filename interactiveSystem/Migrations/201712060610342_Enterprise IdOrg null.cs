namespace interactiveSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EnterpriseIdOrgnull : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Enterprises", "IdOrg", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Enterprises", "IdOrg", c => c.Int(nullable: false));
        }
    }
}
