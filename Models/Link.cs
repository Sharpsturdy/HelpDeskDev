using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Help_Desk_2.Models
{
    public class Link
    {
        public int linkID { get; set;  }
        public string linkText { get; set;  }
        public string linkURL { get; set;  }

        public string linkBrief { get; set; }
    }
}