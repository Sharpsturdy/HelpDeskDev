using Help_Desk_2.DataAccessLayer;
using Help_Desk_2.Models;
using Help_Desk_2.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Help_Desk_2.Controllers
{
    public class TrashCanController : Controller
    {
        HelpDeskContext db = new HelpDeskContext();

        // GET: TrashCan
        public ActionResult Index()
        {
            if (!AllSorts.userHasRole("AdminUsers"))
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

            ViewBag.news = db.News.Where(x => x.deleted).OrderByDescending(x => x.creationDate).ToList<News>();
            ViewBag.tickets = db.Tickets.Where(x => x.deleted).OrderByDescending(x => x.dateComposed).ToList<Ticket>();
            ViewBag.faqs = db.KnowledgeFAQs.Where(x => x.deleted && x.type == 1).ToList<KnowledgeFAQ>();
            ViewBag.kbs = db.KnowledgeFAQs.Where(x => x.deleted && x.type == 2).ToList<KnowledgeFAQ>();
            return View();
        }

        // GET: TrashCan/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TrashCan/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TrashCan/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: TrashCan/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TrashCan/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: TrashCan/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TrashCan/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
