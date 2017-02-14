using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Postal;
using Help_Desk_2.DataAccessLayer;

namespace Help_Desk_2.BackgroundJobs
{
    public static class Mailer
    {
        public static void sendNotification()
        {
            dynamic email = new Email("Test");
            email.To = EmailAddresses.DeveloperEmail;
            email.Message = "DB.GetRandomLolcatLink()";
            email.Send();
        }

        public static void sendTicketNotification(string mailType, string copyTo, int refNum)
        {

            if(mailType == "SubmitTicket")
            {
                //HelpDeskContext db = new HelpDeskContext();
                string sendTo = "patrice@aloehealthsecrets.com";//string.Join(",", db.UserProfiles.Where(x => x.isResponsible).Select(x => x.emailAddress).ToArray<string>());
                dynamic email = new Email("General");
                email.To = sendTo;
                email.CC = copyTo;
                email.Subject = "New Ticket submitted: " + refNum;
                email.Message = "A new ticket, ref: " + refNum + ", has been submitted for your attention.";
                email.Send();
            }


        }


    }
}