﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Configuration;
using Help_Desk_2.Models;
using System.Data.Entity.ModelConfiguration.Conventions;

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
        public DbSet<KnowledgeFAQ> KnowledgeFAQs { get; set;  }
        public DbSet<AuditTrail> AuditTrails { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        
    }

   
}