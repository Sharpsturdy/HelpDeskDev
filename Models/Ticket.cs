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
        [Display(Name = "Temporary ticket No.")]
        public int ID { get; set;  }
        
        [Display(Name = "Ticket No.")]
        public int ticketID { get; set;  }

        [ForeignKey("UserProfile")]
        public Guid originatorID { get; set; } //AD Username

        [Column(TypeName = "DateTime")]
        [Display(Name = "Created on:"), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        [DataType(DataType.DateTime)]
        public DateTime dateComposed { get; set; }

        [NotMapped]
        public string message { get; set; }

        [Required]
        [Display(Name = "Brief description")]
        [StringLength(150, ErrorMessage = "Your brief description exceeds the maximum characters allowed")]
        public string headerText { get; set; }

        [Required]
        [Display(Name = "Detailed description")]
        [DataType(DataType.MultilineText)]
        public string description { get; set; }

        public ICollection<Link> Links { get; set; }

        //read-only faq approverID
        [Column(TypeName = "DateTime")]
        [DataType(DataType.DateTime)]
        public DateTime? dateSubmitted { get; set; }
        public string adminEmail { get; set; }

        [Column(TypeName = "DateTime")]
        [DataType(DataType.DateTime)]
        public DateTime? dateL1Release { get; set; }

        [Column(TypeName = "DateTime")]
        [DataType(DataType.DateTime)]
        public DateTime? dateL2Release { get; set; }
                
        [NotMapped]
        [Display(Name = "Status")]
        public Statuses? status {
            get { if (dateSubmitted == DateTime.MinValue) { return Statuses.Draft ; }; return Statuses.Submitted; }
            set {
                status = value;
            }
        }

        public virtual UserProfile UserProfile { get; set; }

        public virtual ICollection<Attachment> Files { get; set;  }

        //View model public string message { get; set; }

        [NotMapped]
        [Display(Name = "Choose file")]
        [StringLength(150, ErrorMessage = "Your brief description exceeds the maximum characters allowed")]
        public string filePath { get; set; }




        public string sanityCheck { get; set; }


    }
}