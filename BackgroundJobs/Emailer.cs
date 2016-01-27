using Postal;
using System;
using System.Linq;
using Help_Desk_2.DataAccessLayer;
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

        private HelpDeskContext db;

        public void sendNotification()
        {
            dynamic email = new Email("Test");
            email.To = "pelias@avexacomputing.net";
            email.Message = "DB.GetRandomLolcatLink()";
            email.Send();
        }


        public void sendTicketNotification(string mailType, int id)
        {

            if (mailType == "SubmitTicket")
            {
                //HelpDeskContext db = new HelpDeskContext();
                //string sendTo = "webmaster@mispo.org,patrice@aloehealthsecrets.com";//string.Join(",", db.UserProfiles.Where(x => x.isResponsible).Select(x => x.emailAddress).ToArray<string>());

                Ticket t = db.Tickets.Find(id);

                if (t == null) return;

                string approvers = string.Join(",", db.UserProfiles.Where(u => u.isResponsible).Select(x => x.emailAddress).ToArray()).Trim();

                GeneralEmail gemail = new GeneralEmail
                {
                    To = approvers,
                    CC = t.Originator.emailAddress,
                    Subject = "New Ticket submitted: " + t.ID,
                    Message = "A new ticket, ref: " + t.ID + ", has been submitted for your attention."
                };
                gemail.Send();
            }
        }

        public void sendFAQKBNotification(string mailType, int id)
        {

            if (mailType == "Approved")
            {
                //HelpDeskContext db = new HelpDeskContext();
                //string sendTo = "webmaster@mispo.org,patrice@aloehealthsecrets.com";//string.Join(",", db.UserProfiles.Where(x => x.isResponsible).Select(x => x.emailAddress).ToArray<string>());

                KnowledgeFAQ kf = db.KnowledgeFAQs.Find(id);
                
                if (kf == null) return;

                string approvers = string.Join(",", db.UserProfiles.Where(u => u.isKbApprover).Select(x => x.emailAddress).ToArray()).Trim();
                
                GeneralEmail gemail = new GeneralEmail
                {
                    To = kf.Originator.emailAddress,
                    Subject = "Knowledge base article approved!",

                    Message = "<p>Your article '" + kf.headerText + "'has been approved.</p>"
                    
                };

                if (!string.IsNullOrEmpty(approvers))
                    gemail.CC = approvers;

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
            db = new HelpDeskContext();
        }
    }
}