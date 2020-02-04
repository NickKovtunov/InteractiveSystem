namespace interactiveSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTypeandOrderinEnterpriseModelandRewardModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Enterprises", "Type", c => c.Int(nullable: false));
            AddColumn("dbo.Enterprises", "Order", c => c.Int(nullable: false));
            AddColumn("dbo.Rewards", "Type", c => c.Int(nullable: false));
            AddColumn("dbo.Rewards", "Order", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Rewards", "Order");
            DropColumn("dbo.Rewards", "Type");
            DropColumn("dbo.Enterprises", "Order");
            DropColumn("dbo.Enterprises", "Type");
        }
    }
}
