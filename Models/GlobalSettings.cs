using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Help_Desk_2.Models
{
    public class GlobalSettings
    {
        public Guid ID { get; set; }

        public string AdminEmail { get; set; }
        //public Enumerable ResposiblePersons 

        public int TicketSeeder { get; set; }

        public string FAQApprover { get; set; }
        public string KBApprover { get; set; }

        public string TicketHeader { get; set; }

        public string Keyowrds { get; set; } //Change to list
        public string ExpertArea { get; set;  } //Change to list

        public DateTime TicketExpiry { get; set; }

        //public Enumation LanguageOptions { get; set; }

    }
}