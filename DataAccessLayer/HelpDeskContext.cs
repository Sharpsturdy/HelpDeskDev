using System;
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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }

   
}