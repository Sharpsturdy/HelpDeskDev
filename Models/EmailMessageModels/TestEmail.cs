using Postal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Help_Desk_2.Models.EmailMessageModels
{
    public class TestEmail:Email
    {
        public string To { get; set; }
        public string From { get; set; }
        public string Message { get; set; }
    }
}