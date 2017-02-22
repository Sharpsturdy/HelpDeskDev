using Postal;
using System;
using System.Linq;
using Help_Desk_2.DataAccessLayer;
using Help_Desk_2.Models;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web.Hosting;
using System.IO;
using System.Web.Mvc;
using Help_Desk_2.Models.EmailMessageModels;
using System.Web;

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

        private readonly HelpDeskContext db;
        private readonly EmailService _emailServcie;
        private string From
        {
            get
            {
                string tmp = ConfigurationManager.AppSettings["FromAddress"];
                if (string.IsNullOrEmpty(tmp?.Trim()))
                {
                    return "helpdesk@renold.com";
                }
                else
                {
                    return tmp.Trim();
                }
            }
        }

        public Emailer()
        {
            db = new HelpDeskContext();
            _emailServcie = SetupEmailService();    

        }

        private EmailService SetupEmailService()
        {

            var viewsPath = Path.GetFullPath(HostingEnvironment.MapPath(@"~/Views/Emails"));
            var engines = new ViewEngineCollection();
            engines.Add(new FileSystemRazorViewEngine(viewsPath));
            return new EmailService(engines);
        }

        public void sendNotification1()
        {
            dynamic email = new Email("KB");
            email.From = From;
            email.To = EmailAddresses.DeveloperEmail;
            
            email.Send();
        }

        public void sendNotification()
        {
            var email = new TestEmail();
            email.To = EmailAddresses.DeveloperEmail;
            email.From = EmailAddresses.FromNoReplayEmail;
            email.Message = "Hello there!";
           
           _emailServcie.Send(email);
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
            notifySubscribers("FAQs");
            notifySubscribers("KnowledgeBase");
        }

        private void notifySubscribers(string type)
        {
            byte selectFor = 0;
            switch (type)
            {
                case "FAQs":
                    selectFor = 1;
                    break;
                case "KnowledgeBase":
                    selectFor = 2;
                    break;
                default:
                    break;
            }

            var subscrToNotify = GetSubscriptionsForNotification(selectFor);//1-Faq
            foreach (var subscriptions in subscrToNotify)
            {
                var subscriber = subscriptions.Key;
                _sendSubsToUser(type, subscriber, subscriptions);
            }

            UpdateArticleStatusAfterSubscribersNotification(subscrToNotify);
        }

        private void UpdateArticleStatusAfterSubscribersNotification(IEnumerable<IGrouping<Subscriber, Subscription>> faqsubs)
        {
            var notifiedArticleIds = faqsubs.SelectMany(f => f).Select(f => f.FaqId).Distinct();
            if (notifiedArticleIds.Any())
            {
                foreach (var article in db.KnowledgeFAQs.Where(a => notifiedArticleIds.Contains(a.ID)))
                {
                    article.notifiedSubscriptions = true;
                }
                db.SaveChanges();
            }
        }

        private IEnumerable<IGrouping<Subscriber, Subscription>> GetSubscriptionsForNotification(byte articleType)
        {
            var faqSubsLinq = (from faq in db.KnowledgeFAQs
                               where faq.published && !faq.deleted && !faq.processed && faq.type == articleType && !faq.suggest && !faq.notifiedSubscriptions

                               from wl in faq.wordList
                               where !wl.deleted

                               from user in wl.FAQSubs
                               where user.emailAddress.Contains("@")

                               select new
                               {
                                   LoginName = user.loginName,
                                   UserFirstName = user.firstName,
                                   UserSurName = user.surName,
                                   EmailAddress = user.emailAddress,
                                   FaqId = faq.ID,
                                   FaqHeader = faq.headerText,
                                   Originator = faq.Originator.displayName,
                                   Date = faq.dateComposed,
                                   SubscribeFor = wl.type                                   
                               })
                               .GroupBy(s => new Subscriber() {EmailAddress = s.EmailAddress, LoginName = s.LoginName, UserFirstName =  s.UserFirstName, UserSurName = s.UserSurName },
                                        s => new Subscription { FaqHeader = s.FaqHeader, FaqId = s.FaqId, SubscribeFor = s.SubscribeFor, Date = s.Date, OriginatorName = s.Originator }).ToArray();
            return faqSubsLinq;
        }

        private void _sendSubsToUser(string type, Subscriber subscriber, IEnumerable<Subscription> subscriptions)
        {
            var subscriptionEmail = new SubscriptionsEmail();
            subscriptionEmail.From = From;
            subscriptionEmail.To = subscriber.EmailAddress;
            subscriptionEmail.UserName = $"{subscriber.UserFirstName} {subscriber.UserSurName}";
            subscriptionEmail.Type = type;
            subscriptionEmail.Subject = type + " Subscriptions feed";
            subscriptionEmail.Subscribtions = subscriptions;
            subscriptionEmail.BaseURL = ConfigurationManager.AppSettings["BaseURL"];
            _emailServcie.Send(subscriptionEmail);
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
            string articleType = kf.GetArticleTypeString();
            var email = new FaqKbEmail(articleType);
            email.From = From;
            email.Type = mailType;
            email.Title = kf.headerText;
            email.Id = "" + kf.ID;
            email.User = kf.Originator.displayName;
            email.Cc = null;
            string baseUrl    = ConfigurationManager.AppSettings["BaseURL"];
            email.ArticleUrl  = $"{baseUrl}/{articleType}/Details/{kf.ID}";

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

            _emailServcie.Send(email);
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
            email.To = EmailAddresses.DeveloperEmail;
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
                
                email.To = EmailAddresses.DeveloperEmail;
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