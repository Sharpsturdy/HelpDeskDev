using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Help_Desk_2.Models
{
    public class WordList
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "This field cannot be blank. A value is required")]
        [StringLength(100, ErrorMessage = "You have exceeded the maximum characters allowed")]
        public string text { get; set; }
        public int type { get; set; } //type => 1=Keyword, 2=ExpertArea,3= Title,etc

        [DefaultValue(false)]
        public bool deleted { get; set; }

        public virtual ICollection<KnowledgeFAQ> knowledgeFAQ { get; set;  }

        public virtual ICollection<Ticket> ticket { get; set; }

        public virtual ICollection<UserProfile> users { get; set; }
    }
}