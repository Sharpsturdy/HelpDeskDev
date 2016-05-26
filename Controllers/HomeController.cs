using Help_Desk_2.BackgroundJobs;
using Help_Desk_2.DataAccessLayer;
using Help_Desk_2.Models;
using Help_Desk_2.Utilities;
using Postal;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Help_Desk_2.Controllers
{
    public class HomeController : Controller
    {
        private HelpDeskContext db = new HelpDeskContext();

        public ActionResult Index()
        {
            ViewBag.news = (from row in db.News
                       where row.published
                       orderby row.sticky descending, row.publishedDate descending
                       select row).Take(5).ToList();

            ViewBag.faqs = (db.KnowledgeFAQs.Where(k => k.type == 1 && !k.suggest && k.published)
                    .OrderByDescending(k => k.dateComposed)).Take(5).ToList();

            UserProfile up = db.UserProfiles.Find(new Guid(AllSorts.getUserID()));

            ViewBag.faqsubskw = up.faqKeywords.Select(x=>x.text);
            ViewBag.faqsubsea = up.faqExpertAreas.Select(x => x.text);

            ViewBag.kbsubskw = up.kbKeywords.Select(x => x.text);
            ViewBag.kbsubsea = up.kbExpertAreas.Select(x => x.text);

            return View();
        }

        public ActionResult UnAuthorized()
        {
            return View();
        }

        public ActionResult TestSend()
        {
            return View();
        }

        [HttpPost]
        public string TestSend(int? id) {
            

            Hangfire.BackgroundJob.Enqueue<Emailer>(x => x.sendTicketNotification("SubmitTicket", 11));
            return "duet";

        }


    }
}