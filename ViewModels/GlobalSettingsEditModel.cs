using Help_Desk_2.Models;
using Help_Desk_2.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using System.Linq;

namespace Help_Desk_2.ViewModels
{
    public class GlobalSettingsEditModel
    {
        public Guid ID { get; set; }

        [Required(ErrorMessage = "Enter a valid email address")]
        [Display(Name = "Admin Email")]
        [EmailAddress]
        public string AdminEmail { get; set; }

        //public Enumerable ResposiblePersons 

        [Required(ErrorMessage = "Enter the next ticket number to use")]
        [Display(Name = "Next Ticket No.")]
        [DefaultValue(1000)]
        public int TicketSeeder { get; set; }
        
        [Display(Name = "FAQs Approver")]
        public string FAQApprover { get; set; }

        [Display(Name = "Knowledgebase Approver")]
        public string KBApprover { get; set; }

        [Display(Name = "New ticker message")]
        [DataType(DataType.MultilineText)]
        public string TicketHeader { get; set; }

        public List<string> Keywords {
            get
            {
                return AllSorts.FullWordList.Where(x => x.type == 1).OrderBy(x=>x.text).Select(x => x.text).ToList<string>();
            }
        } //Change to list

        [Display(Name = "Expert Areas")]
        public List<string> ExpertAreas {
            get
            {
                return AllSorts.FullWordList.Where(x => x.type == 2).OrderBy(x => x.text).Select(x => x.text).ToList<string>();
            }
        }  //Change to list

        [Required]
        [Display(Name = "Ticket Expiry Days")]
        public int TicketExpiryDays { get; set; }

        [Required]
        [Display(Name = "FAQs/Knowledge Base Expiry Days")]
        public int KBFAQsExpiryDays { get; set; }

        //public Enumation LanguageOptions { get; set; }

        public GlobalSettingsEditModel() { }

        public GlobalSettingsEditModel(GlobalSettings gs)
        {
            this.ID = gs.ID;
            this.AdminEmail = gs.AdminEmail;
            this.TicketSeeder = gs.TicketSeeder;
            this.FAQApprover = gs.FAQApprover;
            this.KBApprover = gs.KBApprover;
            this.TicketHeader = gs.TicketHeader;
            this.TicketExpiryDays = gs.TicketExpiryDays;
            this.KBFAQsExpiryDays = gs.KBFAQsExpiryDays;
        }

        public GlobalSettings getSettings()
        {
            GlobalSettings gs = new GlobalSettings();
            gs.ID = this.ID;
            gs.AdminEmail = this.AdminEmail;
            gs.TicketSeeder = this.TicketSeeder;
            gs.FAQApprover = this.FAQApprover;
            gs.KBApprover = this.KBApprover;
            gs.TicketHeader = this.TicketHeader;
            gs.TicketExpiryDays = this.TicketExpiryDays;
            gs.KBFAQsExpiryDays = this.KBFAQsExpiryDays;

            return gs;
        }
    }
}