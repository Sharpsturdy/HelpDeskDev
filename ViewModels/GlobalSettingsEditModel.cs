using Help_Desk_2.Models;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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

        public string Keyowrds { get; set; } //Change to list

        [Display(Name = "Expert Areas")]
        public string ExpertArea { get; set; } //Change to list

        public DateTime TicketExpiry { get; set; }

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
            this.Keyowrds = gs.Keyowrds;
            this.ExpertArea = gs.ExpertArea;
            this.TicketExpiry = gs.TicketExpiry;
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
            gs.Keyowrds = this.Keyowrds;
            gs.ExpertArea = this.ExpertArea;
            gs.TicketExpiry = this.TicketExpiry;

            return gs;
        }
    }
}