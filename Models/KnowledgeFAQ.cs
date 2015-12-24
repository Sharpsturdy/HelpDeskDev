using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
        public string description { get; set; }
        
        //public virtual Attachments { get; set;  }

        //public virtual keywords

        //public virtual expert area
        public string links { get; set; }

        //read-only faq approverID

    }
}