using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Help_Desk_2.Models
{
    public class WordList
    {
        public int ID { get; set; }
        public string text { get; set; }
        public int type { get; set; } //type => 1=Keyword, 2=ExpertArea,3= Title,etc

        public virtual ICollection<KnowledgeFAQ> knowledgeFAQ { get; set;  }

        public virtual ICollection<KnowledgeFAQ> ticket { get; set; }
    }
}