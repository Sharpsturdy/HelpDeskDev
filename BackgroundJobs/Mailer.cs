using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Postal;

namespace Help_Desk_2.BackgroundJobs
{
    public static class Mailer
    {
        public static void sendNotification()
        {
            dynamic email = new Email("Test");
            email.To = "pelias@avexacomputing.net";
            email.Message = "DB.GetRandomLolcatLink()";
            email.Send();
        }
    }
}