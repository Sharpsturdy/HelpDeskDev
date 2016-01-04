using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Configuration;
using Help_Desk_2.Models;
using System.Data.Entity.ModelConfiguration.Conventions;
using Help_Desk_2.ViewModels;

namespace Help_Desk_2.DataAccessLayer
{
    public class HelpDeskContext : DbContext
    {
        public HelpDeskContext() :base("HelpDeskContext")
        {
        }

        public DbSet<GlobalSettings> GlobalSettingss { get; set;  }
        public DbSet<WordList> WordLists { get; set;  }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<News> News { get; set; }

        public DbSet<WordListViewModel> WordListViewModels { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            /*
            modelBuilder.Entity<Ticket>()
                .HasMany(a => a.Files)
                .WithOptional(t => t.Ticket)
                .HasForeignKey(fk => new { fk.ID, fk.parentID });
            */
            /*
            modelBuilder.Entity<Attachment>()
                .HasKey(k => new { k.ID, k.attachType})
                .HasRequired(t => t.Ticket)
                .WithMany(t => t.Files)
                .HasForeignKey(t => new { t.parentID, t.attachType });
            */
            /*
            modelBuilder.Entity<Ticket>()
                .HasOptional( t => t)
            */
            /*
            modelBuilder.Entity<Attachment>()
                .HasRequired(t => t.Ticket)
                .WithMany(a => a.Files)
                .Map()
                //.HasForeignKey(fk => new { fk.parentID, fk.attachType });
            */    
        }

        public System.Data.Entity.DbSet<Help_Desk_2.Models.KnowledgeFAQ> KnowledgeFAQs { get; set; }
    }

   
}