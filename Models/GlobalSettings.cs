using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Help_Desk_2.Utilities;
using System.ComponentModel;

namespace Help_Desk_2.Models
{
    public class GlobalSettings
    {
        public Guid ID { get; set; }

        
        /*[NotMapped]
        [Required(ErrorMessage = "Select one or more names")]
        [Display(Name = "Ticket Admin Email(s)")]
        public string AdminEmail { get; set; }*/

        [Required(ErrorMessage = "Enter the next ticket number to use")]
        [Display(Name = "Next Ticket No.")]
        [DefaultValue(1000)]
        public int TicketSeeder { get; set; }
               
        
        [Display(Name = "New ticker message")]
        [DataType(DataType.MultilineText)]
        public string TicketHeader { get; set; }

        [NotMapped]
        public List<string> Keywords
        {
            get
            {
                return AllSorts.FullWordList.Where(x => x.type == 1).Select(x => x.text).ToList<string>();
            }
        } 
        
        [NotMapped]
        [Display(Name = "Expert Areas")]
        public List<string> ExpertAreas
        {
            get
            {
                return AllSorts.FullWordList.Where(x => x.type == 2).Select(x => x.text).ToList<string>();
            }
        }  

        [Required]
        [Display(Name = "Ticket Expiry Days")]
        public int TicketExpiryDays { get; set; }

        [Required]
        [Display(Name = "FAQs Expiry Days")]
        public int FAQsExpiryDays { get; set; }

        [Required]
        [Display(Name = "Knowledgebase Expiry Days")]
        public int KBExpiryDays { get; set; }

        [Display(Name = "Enabled")]
        [DefaultValue(false)]
        public bool TicketHeaderEnabled { get; set; }

    }
}