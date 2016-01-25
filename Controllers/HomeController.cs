using Help_Desk_2.DataAccessLayer;
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
        private HelpDeskContext db = new HelpDeskContext();

        public ActionResult Index()
        {
            ViewBag.news = (from row in db.News
                       where row.published
                       orderby row.sticky descending, row.publishedDate descending
                       select row).Take(5).ToList();

            ViewBag.faqs = (db.KnowledgeFAQs.Where(k => k.type == 1 && !k.suggest && k.published)
                    .OrderByDescending(k => k.dateComposed)).Take(5).ToList();

            return View();
        }

        public ActionResult UnAuthorized()
        {
            return View();
        }

        
        
    }
}