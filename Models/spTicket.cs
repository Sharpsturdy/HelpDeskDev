using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Help_Desk_2.Models
{
    public class spTicket
    {
        public string yr { get; set; }
        public int monNum { get; set; }
        public string status { get; set; }
        public string displayName { get; set; }

        [DefaultValue(0)]
        public int total { get; set; }
    }
}