namespace interactiveSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RewardIdRewnull : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Rewards", "IdRew", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Rewards", "IdRew", c => c.Int(nullable: false));
        }
    }
}
