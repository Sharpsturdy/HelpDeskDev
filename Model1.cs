namespace Help_Desk_2
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Model1 : DbContext
    {
        public Model1()
            : base("name=Model1")
        {
        }

        public virtual DbSet<Attachment> Attachment { get; set; }
        public virtual DbSet<AuditTrail> AuditTrail { get; set; }
        public virtual DbSet<GlobalSettings> GlobalSettings { get; set; }
        public virtual DbSet<KnowledgeFAQ> KnowledgeFAQ { get; set; }
        public virtual DbSet<News> News { get; set; }
        public virtual DbSet<Ticket> Ticket { get; set; }
        public virtual DbSet<UserProfile> UserProfile { get; set; }
        public virtual DbSet<WordList> WordList { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<KnowledgeFAQ>()
                .HasMany(e => e.Attachment)
                .WithOptional(e => e.KnowledgeFAQ)
                .HasForeignKey(e => e.commonID);

            modelBuilder.Entity<KnowledgeFAQ>()
                .HasMany(e => e.WordList)
                .WithMany(e => e.KnowledgeFAQ)
                .Map(m => m.ToTable("WordListKnowledgeFAQ"));

            modelBuilder.Entity<Ticket>()
                .HasMany(e => e.Attachment)
                .WithOptional(e => e.Ticket)
                .HasForeignKey(e => e.parentID);

            modelBuilder.Entity<Ticket>()
                .HasMany(e => e.AuditTrail)
                .WithRequired(e => e.Ticket)
                .HasForeignKey(e => e.refID);

            modelBuilder.Entity<Ticket>()
                .HasMany(e => e.WordList)
                .WithMany(e => e.Ticket)
                .Map(m => m.ToTable("TicketWordList"));

            modelBuilder.Entity<UserProfile>()
                .HasMany(e => e.KnowledgeFAQ)
                .WithRequired(e => e.UserProfile)
                .HasForeignKey(e => e.originatorID);

            modelBuilder.Entity<UserProfile>()
                .HasMany(e => e.News)
                .WithRequired(e => e.UserProfile)
                .HasForeignKey(e => e.originatorID);

            modelBuilder.Entity<UserProfile>()
                .HasMany(e => e.Ticket)
                .WithRequired(e => e.UserProfile)
                .HasForeignKey(e => e.originatorID);

            modelBuilder.Entity<UserProfile>()
                .HasMany(e => e.Ticket1)
                .WithOptional(e => e.UserProfile1)
                .HasForeignKey(e => e.responsibleID);

            modelBuilder.Entity<UserProfile>()
                .HasMany(e => e.Ticket2)
                .WithOptional(e => e.UserProfile2)
                .HasForeignKey(e => e.UserProfile_userID);

            modelBuilder.Entity<UserProfile>()
                .HasMany(e => e.WordList)
                .WithMany(e => e.UserProfile)
                .Map(m => m.ToTable("UserProfileWordList"));

            modelBuilder.Entity<UserProfile>()
                .HasMany(e => e.WordList1)
                .WithMany(e => e.UserProfile1)
                .Map(m => m.ToTable("UserProfileWordList1"));
        }
    }
}
