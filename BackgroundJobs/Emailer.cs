using Postal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Help_Desk_2.Utilities;
using Help_Desk_2.BackgroundJobs;
using Help_Desk_2.Models;

namespace Help_Desk_2.BackgroundJobs
{
    public class GeneralEmail : Email
    {
        public string To { get; set;  }
        public string Bcc { get; set; }
        public string CC { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }

        
    }
    public class Emailer
    {

        public void sendNotification()
        {
            dynamic email = new Email("Test");
            email.To = "pelias@avexacomputing.net";
            email.Message = "DB.GetRandomLolcatLink()";
            email.Send();
        }

        public void sendTicketNotification(string mailType, string copyTo, int refNum)
        {

            if (mailType == "SubmitTicket")
            {
                //HelpDeskContext db = new HelpDeskContext();
                //string sendTo = "webmaster@mispo.org,patrice@aloehealthsecrets.com";//string.Join(",", db.UserProfiles.Where(x => x.isResponsible).Select(x => x.emailAddress).ToArray<string>());
                GeneralEmail gemail = new GeneralEmail
                {
                    To = copyTo,
                    CC = "patrice@aloehealthsecrets.com",
                    Subject = "New Ticket submitted: " + refNum,
                    Message = "A new ticket, ref: " + refNum + ", has been submitted for your attention."
                };
                gemail.Send();
            }
        }

        public string sendThis()
        {
            try
            {
                int refNum = 102;
               
                dynamic email = new Email("General");
                
                email.To = "pelias@avexacomputing.net";
                email.CC = "patrice@aloehealthsecrets.com";
                email.Subject = "New Ticket submitted: " + refNum;
                email.Message = "A new ticket, ref: " + refNum + ", has been submitted for your attention.";
                email.Send();

                return "Message sent";
            }
            catch (Exception e)
            {
                return "Message failed because: " + e.Message;
            }
        }


        public Emailer()
        {

        }
    }
}