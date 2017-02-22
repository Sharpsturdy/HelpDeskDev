using Postal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Help_Desk_2.Models.EmailMessageModels
{
    public class SubscriptionsEmail:Email
    {
        public string BaseURL { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public IEnumerable<Subscription> Subscribtions { get; set; }
        public string To { get; set; }
        public string Type { get; set; }
        public string UserName { get; set; }
    }
}