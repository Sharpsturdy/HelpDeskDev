using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using System.DirectoryServices.AccountManagement;
using Help_Desk_2.Utilities;
using Help_Desk_2.DataAccessLayer;

namespace Help_Desk_2.Models
{
    public enum Statuses
    {
        Draft, Submitted, Assigned, OnHold, Completed
    }
    public class Ticket
    {
        public int ID { get; set;  }
        public int ticketID { get; set;  }

        [ForeignKey("UserProfile")]
        public Guid originatorID { get; set; } //AD Username

        /*[NotMapped]
        public string originatorFullname {
            get
            {
                if (originatorUsername == "")
                    return "Unknown User";

                try {
                    HelpDeskContext db = new HelpDeskContext();
                    
                    //Grab current user from profile
                    UserProfile up = db.UserProfiles.Find(new Guid(originatorUsername));
                    return up.displayName;
                } catch(Exception ex) {
                    return "Unknown User1";
                }
            }
         } //AD Username
        */
        [Column(TypeName = "DateTime")]
        [Display(Name = "Created on:"), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime dateComposed { get; set; }

        [NotMapped]
        public string message { get; set; }
        public string headerText { get; set; }
        public string description { get; set; }

        //public virtual Attachments { get; set;  }
        public ICollection<Link> Links { get; set; }

        //read-only faq approverID
        [Column(TypeName = "DateTime")]
        public DateTime? dateSubmitted { get; set; }
        public string adminEmail { get; set; }

        [Column(TypeName = "DateTime")]
        public DateTime? dateL1Release { get; set; }

        [Column(TypeName = "DateTime")]
        public DateTime? dateL2Release { get; set; }

        public string sanityCheck { get; set;  }

        [NotMapped]
        public Statuses? status {
            get { if (dateSubmitted == DateTime.MinValue) { return Statuses.Draft ; }; return Statuses.Submitted; }
            set {
                status = value;
            }
        }

        public virtual UserProfile UserProfile { get; set; }

    }
}