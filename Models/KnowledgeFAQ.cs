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

        [Display(Name = "Created on:"), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
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

        [Display(Name = "Links")]
        public string links { get; set; }

        [NotMapped]
        public string linkText { get; set; }

        [NotMapped]
        public string linkURL { get; set; }

        public virtual UserProfile UserProfile { get; set; }

        public virtual ICollection<Attachment> Files { get; set; }

        

        //public virtual keywords

       

        //read-only faq approverID

    }
}