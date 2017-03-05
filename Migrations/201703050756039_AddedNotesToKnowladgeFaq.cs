namespace Help_Desk_2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedNotesToKnowladgeFaq : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.KnowledgeFAQ", "notes", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.KnowledgeFAQ", "notes");
        }
    }
}
