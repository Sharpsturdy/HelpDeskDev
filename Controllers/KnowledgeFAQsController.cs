using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Help_Desk_2.DataAccessLayer;
using Help_Desk_2.Models;

namespace Help_Desk_2.Controllers
{
    public class KnowledgeFAQsController : Controller
    {
        private HelpDeskContext db = new HelpDeskContext();

        // GET: KnowledgeFAQs
        public ActionResult Index()
        {
            var knowledgeFAQs = db.KnowledgeFAQs.Include(k => k.UserProfile);
            return View(knowledgeFAQs.ToList());
        }

        // GET: KnowledgeFAQs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KnowledgeFAQ knowledgeFAQ = db.KnowledgeFAQs.Find(id);
            if (knowledgeFAQ == null)
            {
                return HttpNotFound();
            }
            return View(knowledgeFAQ);
        }

        // GET: KnowledgeFAQs/Create
        public ActionResult Create()
        {
            ViewBag.originatorID = new SelectList(db.UserProfiles, "userID", "loginName");
            return View();
        }

        // POST: KnowledgeFAQs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,originatorUsername,originatorFullname,dateComposed,exiryDate,headerText,description,originatorID,links")] KnowledgeFAQ knowledgeFAQ)
        {
            if (ModelState.IsValid)
            {
                db.KnowledgeFAQs.Add(knowledgeFAQ);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.originatorID = new SelectList(db.UserProfiles, "userID", "loginName", knowledgeFAQ.originatorID);
            return View(knowledgeFAQ);
        }

        // GET: KnowledgeFAQs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KnowledgeFAQ knowledgeFAQ = db.KnowledgeFAQs.Find(id);
            if (knowledgeFAQ == null)
            {
                return HttpNotFound();
            }
            ViewBag.originatorID = new SelectList(db.UserProfiles, "userID", "loginName", knowledgeFAQ.originatorID);
            return View(knowledgeFAQ);
        }

        // POST: KnowledgeFAQs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,originatorUsername,originatorFullname,dateComposed,exiryDate,headerText,description,originatorID,links")] KnowledgeFAQ knowledgeFAQ)
        {
            if (ModelState.IsValid)
            {
                db.Entry(knowledgeFAQ).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.originatorID = new SelectList(db.UserProfiles, "userID", "loginName", knowledgeFAQ.originatorID);
            return View(knowledgeFAQ);
        }

        // GET: KnowledgeFAQs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KnowledgeFAQ knowledgeFAQ = db.KnowledgeFAQs.Find(id);
            if (knowledgeFAQ == null)
            {
                return HttpNotFound();
            }
            return View(knowledgeFAQ);
        }

        // POST: KnowledgeFAQs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            KnowledgeFAQ knowledgeFAQ = db.KnowledgeFAQs.Find(id);
            db.KnowledgeFAQs.Remove(knowledgeFAQ);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
