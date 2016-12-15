using Help_Desk_2.DataAccessLayer;
using Help_Desk_2.Models;
using Help_Desk_2.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MvcPaging;
using Help_Desk_2.BackgroundJobs;

namespace Help_Desk_2.Controllers
{
    public class SearchController : Controller
    {

        private HelpDeskContext db = new HelpDeskContext();
        // GET: Search
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Search(string searchStr, int? page)
        {
            var faqs = from m in db.KnowledgeFAQs
                       where (m.type == 1 && !m.suggest && m.published)
                       select m;
            var news = from n in db.News
                       where (n.published)
                       select n;

            if (!String.IsNullOrEmpty(searchStr))
            {
                faqs = faqs.Where(s => s.headerText.Contains(searchStr) || s.description.Contains(searchStr));
            }

            if (!String.IsNullOrEmpty(searchStr))
            {
                news = news.Where(s => s.body.Contains(searchStr) || s.title.Contains(searchStr));
            }


            int currentPageIndex1 = page.HasValue ? page.Value - 1 : 0;
            ViewBag.NewsResults = news.OrderByDescending(m => m.creationDate).ToPagedList(currentPageIndex1, AllSorts.pageSize);

            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            ViewBag.FAQResults = faqs.OrderByDescending(m => m.dateComposed).ToPagedList(currentPageIndex, AllSorts.pageSize);
            return View("Index");

        }
    }
}