namespace Help_Desk_2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GlobalSettings",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        AdminEmail = c.String(),
                        TicketSeeder = c.Int(nullable: false),
                        FAQApprover = c.String(),
                        KBApprover = c.String(),
                        TicketHeader = c.String(),
                        Keyowrds = c.String(),
                        ExpertArea = c.String(),
                        TicketExpiry = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.GlobalSettings");
        }
    }
}
