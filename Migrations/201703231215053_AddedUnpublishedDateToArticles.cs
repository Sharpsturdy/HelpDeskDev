namespace Help_Desk_2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedUnpublishedDateToArticles : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.KnowledgeFAQ", "dateUnpublished", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.KnowledgeFAQ", "dateUnpublished");
        }
    }
}
