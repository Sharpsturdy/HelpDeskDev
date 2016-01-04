using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Help_Desk_2.Models
{
    public class KnowledgeFAQ
    {
        public int ID { get; set;  }

        [Display(Name = "Created on:"), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        [DataType(DataType.DateTime)]
        public DateTime dateComposed { get; set; }

        [Display(Name = "Number of days to expiry:"),DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        [DataType(DataType.DateTime)]
        public DateTime? exiryDate { get; set;  }

        [Required]
        [Display(Name = "Brief description")]
        [StringLength(150, ErrorMessage = "Your brief description exceeds the maximum characters allowed")]
        public string headerText { get; set; }

        [Required]
        [AllowHtml]
        [Display(Name = "Detailed description")]
        [DataType(DataType.MultilineText)]
        public string description { get; set; }

        [ForeignKey("UserProfile")]
        public Guid originatorID { get; set; } //AD Username
        
        public virtual UserProfile UserProfile { get; set; }

        public virtual ICollection<Attachment> Files { get; set; }
                
        [Display(Name = "Links")]
        public string links { get; set; }

        [NotMapped]
        public string linkText { get; set; }

        [NotMapped]
        public string linkURL { get; set; }

        [NotMapped]     //List of files to delete
        public string deleteField { get; set; }

        public byte type { get; set; } //type => 1=FAQs, 2=Knowledge Base

        [NotMapped]
        public string status { get
            {
                return "Draft";
            }
        }

        //read-only faq approverID

        //read-only kb approverID

    }
}