namespace Help_Desk_2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddHelp : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Attachment",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        parentID = c.Int(),
                        commonID = c.Int(),
                        newsID = c.Int(),
                        fileName = c.String(nullable: false),
                        filePath = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.KnowledgeFAQ", t => t.commonID)
                .ForeignKey("dbo.News", t => t.newsID)
                .ForeignKey("dbo.Ticket", t => t.parentID)
                .Index(t => t.parentID)
                .Index(t => t.commonID)
                .Index(t => t.newsID);
            
            CreateTable(
                "dbo.KnowledgeFAQ",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        dateComposed = c.DateTime(nullable: false),
                        expiryDate = c.DateTime(),
                        dateSubmitted = c.DateTime(),
                        headerText = c.String(nullable: false, maxLength: 150),
                        description = c.String(nullable: false),
                        originatorID = c.Guid(nullable: false),
                        suggest = c.Boolean(nullable: false),
                        published = c.Boolean(nullable: false),
                        archived = c.Boolean(nullable: false),
                        deleted = c.Boolean(nullable: false),
                        processed = c.Boolean(nullable: false),
                        links = c.String(),
                        type = c.Byte(nullable: false),
                        archiveID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.UserProfile", t => t.originatorID, cascadeDelete: true)
                .Index(t => t.dateComposed)
                .Index(t => t.headerText)
                .Index(t => t.originatorID);
            
            CreateTable(
                "dbo.UserProfile",
                c => new
                    {
                        userID = c.Guid(nullable: false),
                        loginName = c.String(),
                        principalName = c.String(),
                        firstName = c.String(maxLength: 50),
                        surName = c.String(maxLength: 50),
                        emailAddress = c.String(nullable: false),
                        isResponsible = c.Boolean(nullable: false),
                        isFaqApprover = c.Boolean(nullable: false),
                        isKbApprover = c.Boolean(nullable: false),
                        lastSignOn = c.DateTime(),
                        deleted = c.Boolean(nullable: false),
                        displayName = c.String(),
                    })
                .PrimaryKey(t => t.userID);
            
            CreateTable(
                "dbo.WordList",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        text = c.String(nullable: false, maxLength: 100),
                        type = c.Int(nullable: false),
                        deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Ticket",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ticketID = c.Int(nullable: false),
                        originatorID = c.Guid(nullable: false),
                        responsibleID = c.Guid(),
                        dateComposed = c.DateTime(nullable: false),
                        expiryDate = c.DateTime(),
                        dateCompleted = c.DateTime(),
                        deleted = c.Boolean(nullable: false),
                        onhold = c.Boolean(nullable: false),
                        returned = c.Boolean(nullable: false),
                        headerText = c.String(nullable: false, maxLength: 150),
                        description = c.String(nullable: false),
                        links = c.String(),
                        dateSubmitted = c.DateTime(),
                        dateL1Release = c.DateTime(),
                        dateL2Release = c.DateTime(),
                        reason = c.String(),
                        adminComments = c.String(),
                        sanityCheck = c.Int(),
                        summary = c.String(maxLength: 250),
                        report = c.String(),
                        UserProfile_userID = c.Guid(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.UserProfile", t => t.originatorID, cascadeDelete: true)
                .ForeignKey("dbo.UserProfile", t => t.responsibleID)
                .ForeignKey("dbo.UserProfile", t => t.UserProfile_userID)
                .Index(t => t.originatorID)
                .Index(t => t.responsibleID)
                .Index(t => t.UserProfile_userID);
            
            CreateTable(
                "dbo.AuditTrail",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        refID = c.Int(nullable: false),
                        timeStamp = c.DateTime(nullable: false),
                        userID = c.Guid(nullable: false),
                        text = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Ticket", t => t.refID, cascadeDelete: true)
                .Index(t => t.refID);
            
            CreateTable(
                "dbo.News",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        title = c.String(nullable: false, maxLength: 150),
                        body = c.String(nullable: false),
                        sticky = c.Boolean(nullable: false),
                        published = c.Boolean(nullable: false),
                        deleted = c.Boolean(nullable: false),
                        publishedDate = c.DateTime(),
                        originatorID = c.Guid(nullable: false),
                        creationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.UserProfile", t => t.originatorID, cascadeDelete: true)
                .Index(t => t.title)
                .Index(t => t.published)
                .Index(t => t.originatorID);
            
            CreateTable(
                "dbo.GlobalSettings",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        TicketSeeder = c.Int(nullable: false),
                        TicketHeader = c.String(),
                        TicketExpiryDays = c.Int(nullable: false),
                        FAQsExpiryDays = c.Int(nullable: false),
                        KBExpiryDays = c.Int(nullable: false),
                        TicketHeaderEnabled = c.Boolean(nullable: false),
                        HelpFile = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.WordListKnowledgeFAQ",
                c => new
                    {
                        WordList_ID = c.Int(nullable: false),
                        KnowledgeFAQ_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.WordList_ID, t.KnowledgeFAQ_ID })
                .ForeignKey("dbo.WordList", t => t.WordList_ID, cascadeDelete: true)
                .ForeignKey("dbo.KnowledgeFAQ", t => t.KnowledgeFAQ_ID, cascadeDelete: true)
                .Index(t => t.WordList_ID)
                .Index(t => t.KnowledgeFAQ_ID);
            
            CreateTable(
                "dbo.TicketWordList",
                c => new
                    {
                        Ticket_ID = c.Int(nullable: false),
                        WordList_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Ticket_ID, t.WordList_ID })
                .ForeignKey("dbo.Ticket", t => t.Ticket_ID, cascadeDelete: true)
                .ForeignKey("dbo.WordList", t => t.WordList_ID, cascadeDelete: true)
                .Index(t => t.Ticket_ID)
                .Index(t => t.WordList_ID);
            
            CreateTable(
                "dbo.UserProfileWordList",
                c => new
                    {
                        UserProfile_userID = c.Guid(nullable: false),
                        WordList_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserProfile_userID, t.WordList_ID })
                .ForeignKey("dbo.UserProfile", t => t.UserProfile_userID, cascadeDelete: true)
                .ForeignKey("dbo.WordList", t => t.WordList_ID, cascadeDelete: true)
                .Index(t => t.UserProfile_userID)
                .Index(t => t.WordList_ID);
            
            CreateTable(
                "dbo.UserProfileWordList1",
                c => new
                    {
                        UserProfile_userID = c.Guid(nullable: false),
                        WordList_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserProfile_userID, t.WordList_ID })
                .ForeignKey("dbo.UserProfile", t => t.UserProfile_userID, cascadeDelete: true)
                .ForeignKey("dbo.WordList", t => t.WordList_ID, cascadeDelete: true)
                .Index(t => t.UserProfile_userID)
                .Index(t => t.WordList_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Attachment", "parentID", "dbo.Ticket");
            DropForeignKey("dbo.Attachment", "newsID", "dbo.News");
            DropForeignKey("dbo.News", "originatorID", "dbo.UserProfile");
            DropForeignKey("dbo.Attachment", "commonID", "dbo.KnowledgeFAQ");
            DropForeignKey("dbo.KnowledgeFAQ", "originatorID", "dbo.UserProfile");
            DropForeignKey("dbo.Ticket", "UserProfile_userID", "dbo.UserProfile");
            DropForeignKey("dbo.UserProfileWordList1", "WordList_ID", "dbo.WordList");
            DropForeignKey("dbo.UserProfileWordList1", "UserProfile_userID", "dbo.UserProfile");
            DropForeignKey("dbo.UserProfileWordList", "WordList_ID", "dbo.WordList");
            DropForeignKey("dbo.UserProfileWordList", "UserProfile_userID", "dbo.UserProfile");
            DropForeignKey("dbo.TicketWordList", "WordList_ID", "dbo.WordList");
            DropForeignKey("dbo.TicketWordList", "Ticket_ID", "dbo.Ticket");
            DropForeignKey("dbo.Ticket", "responsibleID", "dbo.UserProfile");
            DropForeignKey("dbo.Ticket", "originatorID", "dbo.UserProfile");
            DropForeignKey("dbo.AuditTrail", "refID", "dbo.Ticket");
            DropForeignKey("dbo.WordListKnowledgeFAQ", "KnowledgeFAQ_ID", "dbo.KnowledgeFAQ");
            DropForeignKey("dbo.WordListKnowledgeFAQ", "WordList_ID", "dbo.WordList");
            DropIndex("dbo.UserProfileWordList1", new[] { "WordList_ID" });
            DropIndex("dbo.UserProfileWordList1", new[] { "UserProfile_userID" });
            DropIndex("dbo.UserProfileWordList", new[] { "WordList_ID" });
            DropIndex("dbo.UserProfileWordList", new[] { "UserProfile_userID" });
            DropIndex("dbo.TicketWordList", new[] { "WordList_ID" });
            DropIndex("dbo.TicketWordList", new[] { "Ticket_ID" });
            DropIndex("dbo.WordListKnowledgeFAQ", new[] { "KnowledgeFAQ_ID" });
            DropIndex("dbo.WordListKnowledgeFAQ", new[] { "WordList_ID" });
            DropIndex("dbo.News", new[] { "originatorID" });
            DropIndex("dbo.News", new[] { "published" });
            DropIndex("dbo.News", new[] { "title" });
            DropIndex("dbo.AuditTrail", new[] { "refID" });
            DropIndex("dbo.Ticket", new[] { "UserProfile_userID" });
            DropIndex("dbo.Ticket", new[] { "responsibleID" });
            DropIndex("dbo.Ticket", new[] { "originatorID" });
            DropIndex("dbo.KnowledgeFAQ", new[] { "originatorID" });
            DropIndex("dbo.KnowledgeFAQ", new[] { "headerText" });
            DropIndex("dbo.KnowledgeFAQ", new[] { "dateComposed" });
            DropIndex("dbo.Attachment", new[] { "newsID" });
            DropIndex("dbo.Attachment", new[] { "commonID" });
            DropIndex("dbo.Attachment", new[] { "parentID" });
            DropTable("dbo.UserProfileWordList1");
            DropTable("dbo.UserProfileWordList");
            DropTable("dbo.TicketWordList");
            DropTable("dbo.WordListKnowledgeFAQ");
            DropTable("dbo.GlobalSettings");
            DropTable("dbo.News");
            DropTable("dbo.AuditTrail");
            DropTable("dbo.Ticket");
            DropTable("dbo.WordList");
            DropTable("dbo.UserProfile");
            DropTable("dbo.KnowledgeFAQ");
            DropTable("dbo.Attachment");
        }
    }
}
