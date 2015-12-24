using Help_Desk_2.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Help_Desk_2.ViewModels
{
    public class TicketViewModel
    {
        [Display (Name = "Temporary ticket No.")]
        public int ID { get; set;  }

        [Display(Name = "Ticket No.")]
        public int ticketID { get; set;  }

        public string originatorPN { get; set; } //AD User Principal Name

        [Display(Name ="Created by")]
        public string originatorFullname { get; set; } //AD Username

        [Display(Name = "Created on:"),DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        [DataType(DataType.DateTime)]
        public DateTime dateComposed { get; set; }

        //View model public string message { get; set; }

        [Required]
        [Display(Name = "Brief description")]
        [StringLength(150, ErrorMessage = "Your brief description exceeds the maximum characters allowed")]
        public string headerText { get; set; }

        [Required]
        [Display(Name = "Detailed description")]
        [DataType(DataType.MultilineText)]
        public string description { get; set; }

        //public virtual Attachments { get; set;  }
        public ICollection<Link> links { get; set; }

        //read-only faq approverID
        public DateTime dateSubmitted { get; set; }
        public string adminEmail { get; set; }

        [DataType(DataType.Date)]
        public DateTime dateL1Release { get; set; }

        [DataType(DataType.Date)]
        public DateTime dateL2Release { get; set; }

        public string sanityCheck { get; set;  }

        [Display(Name = "Status")]
        public string status { get; set;  }

       

    }
}