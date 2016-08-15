using Postal;
using System;
using System.Linq;
using Help_Desk_2.DataAccessLayer;
using Help_Desk_2.Models;
using System.Configuration;
using System.Collections;

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

        private string From;

        public Emailer()
        {
            db = new HelpDeskContext();
        
            string tmp = ConfigurationManager.AppSettings["FromAddress"];
            if (tmp.Trim() == "")
            {
                From = "helpdesk@renold.com";
            } else
            {
                From = tmp.Trim();
            }
        }

        public void sendNotification1()
        {
            dynamic email = new Email("KB");
            email.From = From;
            email.To = "pelias@avexacomputing.net";
            
            email.Send();
        }

        public void sendNotification()
        {
            dynamic email = new Email("Test");
            email.To = "pelias@avexacomputing.net";
            email.Message = "DB.GetRandomLolcatLink()";
            email.Send();
        }

        public void sendSubscriptions()
        {
            /****
             * 1. FAQS
             * 2. Send to each user
             * 3. Mark FAQS as processed
             * 
             * 4. Repeat for KBS
             * ********/

            //1. FAQS
            var faqsubs = db.Database.SqlQuery<Subscriptions>("dbo.faqSubs").ToList<Subscriptions>();

            string userName = "", email = "", loginName = "";

            ArrayList subDocs = new ArrayList();
            foreach (var sub in faqsubs)
            {
                if (sub.loginName != loginName)
                {
                    if (loginName != "")
                    {
                        //Send subs for this user
                        _sendSubsToUser("FAQs",userName, email, subDocs);
                    }

                    //Move to next user
                    userName = sub.userName;
                    email = sub.emailAddress;
                    loginName = sub.loginName;
                    subDocs.Clear(); //Reset
                }
                subDocs.Add(sub.ID);
            }
            if (userName != "")
            {
                //Send subs for LAST user
                _sendSubsToUser("FAQs", userName, email, subDocs);

            }

        }

        private void _sendSubsToUser(string type, string userName, string email, ArrayList subDocs)
        {
            dynamic email1 = new Email("Subscriptions");
            email1.From = From;
            email1.To = "patrice.elias@gmail.com";
            email1.userName = userName;
            email1.Type = type;
            email1.Subject = type + " Subscriptions feed";

            //Convert object list to int array
            var tmp = subDocs.ToArray();
            int[] scom = new int[tmp.Length];
            int i = 0;
            foreach (var x in tmp) { scom[i++] = (int)x; }

            //Get full records restricted to matched subs
            email1.Model = db.KnowledgeFAQs.Where(k => k.type == 1 && !k.suggest && k.published && !k.deleted && scom.Contains(k.ID))
                    .OrderByDescending(k => k.dateComposed)
                    .ToList<KnowledgeFAQ>();
           
            email1.Send();


        }
        public void sendTicketNotification(string mailType, int id)
        {
            Ticket tik  = db.Tickets.Find(id); //Get Ticket

            if (tik == null) return;

            dynamic email = new Email("Ticket");
            email.From = From;
            email.type = mailType;
            email.title = tik.headerText;
            email.id = "" + tik.ID;
            email.user = tik.Originator.displayName;

            if (mailType == "Submitted")
            {
                var approvers = from x in db.UserProfiles
                                where x.isResponsible
                                select x;
                
                email.To = string.Join(",", approvers.Select(x => x.emailAddress).ToArray()).Trim();

                email.Subject = "New Ticket Submitted";

            }
            else if (mailType == "Assigned")
            {
                email.To = tik.Responsible.emailAddress;
                email.Subject = "New Ticket Assigned to you [" + tik.ticketID + "]";
            }
            else if (mailType == "Returned")
            {
                var approvers = from x in db.UserProfiles
                                where x.isResponsible
                                select x;

                email.To = string.Join(",", approvers.Select(x => x.emailAddress).ToArray()).Trim();

                email.Subject = "Ticket Returned [" + tik.ticketID + "]";
            }
            else if (mailType == "Completed")
            {
                email.To = tik.Responsible.emailAddress;
                email.Subject = "Ticket Completed [" + tik.ticketID + "]";
            }

            email.Send();

            
        }

        public void sendFAQKBNotification(string mailType, int id)
        {

            KnowledgeFAQ kf = db.KnowledgeFAQs.Find(id); //Get FAQ or KB

            if (kf == null) return;
           
            dynamic email = new Email(kf.type == 1 ? "FAQ":"KB");
            email.From = From;
            email.type = mailType;
            email.title = kf.headerText;
            email.id = "" + kf.ID;
            email.user = kf.Originator.displayName;
             
            if (mailType == "Submitted")
            {
                var approvers = from x in db.UserProfiles
                                select x;

                if (kf.type == 1)
                {
                    approvers = approvers.Where(u => u.isFaqApprover && !string.IsNullOrEmpty(u.emailAddress));
                }
                else if (kf.type == 2)
                {
                    approvers = approvers.Where(u => u.isKbApprover && !string.IsNullOrEmpty(u.emailAddress));
                }
                email.To = string.Join(",", approvers.Select(x => x.emailAddress).ToArray()).Trim();

                email.Subject = "New " + (kf.type == 1 ? "FAQ":"Knowledgebase") + " article submitted";
                
            }
            else if (mailType == "Approved")
            {
                email.To = kf.Originator.emailAddress;
                email.Subject = (kf.type == 1 ? "FAQ" : "Knowledgebase") + " article approved!";
            }

            email.Send();
        }

        /********
         * send ticket reminders every so often as set in global settings if not attended
         * list all tickets submitted but not attend to longer that time set
         * Compose email to set ticket admins and send links to all tickets maybe in a table
         * Ticket header / date submitted / who submitted it / how old the ticket is.
         * **************************/
        public void sendAgedTicketNotification()
        {
            string processingTime = DateTime.Now.ToString();

            dynamic email = new Email("AgedTickets");
            email.From = From;
            email.To = "pelias@avexacomputing.net";
            email.Subject = "Helpdesk System: Aged Tickets Notification";
            email.ticketSLA = "24 hours";

            email.tickets = db.Tickets.Where(t => t.dateSubmitted != null).Take(3).ToList<Ticket>();
            email.Send();

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


       
    }
}