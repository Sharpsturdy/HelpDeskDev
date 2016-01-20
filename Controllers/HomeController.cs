using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Help_Desk_2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult FAQs()
        {
           

            return View();
        }

        public ActionResult KnowledgeBase()
        {
            return View();
        }

        public async Task<string> TestMail()
        {
            try {
                var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
                var message = new MailMessage();
                message.To.Add(new MailAddress("pelias@avexacomputing.net"));  // replace with valid value 
                message.From = new MailAddress("patrice.elias@gmail.com");  // replace with valid value
                message.Subject = "Testing mail send";
                message.Body = string.Format(body, "Patrice M Elias", "Prof P Elias", "The quick brown fox is very lazy");
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = "patrice.elias@gmail.com",  // replace with valid value
                        Password = "Rumb1dz41"  // replace with valid value
                    };
                    smtp.Credentials = credential;
                    //smtp.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["SmtpServerUserName"], ConfigurationManager.AppSettings["SmtpServerPassword"]);
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(message);
                    //return RedirectToAction("Sent");
                    return "Sending message";
                }
            } catch (Exception ex)
            {
                return "<p>Something went wrong</p>" + ex.Message;
            }

        }
        
    }
}