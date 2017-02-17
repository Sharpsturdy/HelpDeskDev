namespace Help_Desk_2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedNotifiedSubscription : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.KnowledgeFAQ", "notifiedSubscriptions", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.KnowledgeFAQ", "notifiedSubscriptions");
        }
    }
}
