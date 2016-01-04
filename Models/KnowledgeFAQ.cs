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
        public string originatorUsername { get; set;  } //AD Username
        public string originatorFullname { get; set; } //AD Username
        public DateTime dateComposed { get; set; }
        public DateTime exiryDate { get; set;  }
        public string headerText { get; set; }

        [AllowHtml]
        [DataType(DataType.MultilineText)]
        public string description { get; set; }

        [ForeignKey("UserProfile")]
        public Guid originatorID { get; set; } //AD Username
        
        public virtual UserProfile UserProfile { get; set; }

        public virtual ICollection<Attachment> Files { get; set; }

        //public virtual keywords

        //public virtual expert area
        public string links { get; set; }

        //read-only faq approverID

    }
}