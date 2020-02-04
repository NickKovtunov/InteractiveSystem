namespace interactiveSystem.Migrations
{
    using Models;
    using System;
    using System.Data.Entity.Migrations;

    public partial class addIdPersonal : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Men", nameof(Man.IdPersonal),
                builder => builder.Guid(nullable: true));
        }

        public override void Down()
        {
            DropColumn("dbo.Men", nameof(Man.IdPersonal));
        }
    }
}
