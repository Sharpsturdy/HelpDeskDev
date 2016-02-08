using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Help_Desk_2.Models
{
    public class AuditTrail
    {
        public Int32 ID { get; set; }

        [ForeignKey("Ticket")]
        public int refID { get; set; }
        
        public DateTime timeStamp { get; set; }
        public Guid userID { get; set; }
        public string text { get; set; }

        public virtual Ticket Ticket { get; set; }

    }
}