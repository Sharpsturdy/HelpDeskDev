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
    public class TicketsAuto1Controller : Controller
    {
        private HelpDeskContext db = new HelpDeskContext();

        // GET: TicketsAuto1
        public ActionResult Index()
        {
            var tickets = db.Tickets.Include(t => t.Responsible).Include(t => t.UserProfile);
            return View(tickets.ToList());
        }

        // GET: TicketsAuto1/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // GET: TicketsAuto1/Create
        public ActionResult Create()
        {
            ViewBag.responsibleID = new SelectList(db.UserProfiles, "userID", "loginName");
            ViewBag.originatorID = new SelectList(db.UserProfiles, "userID", "loginName");
            return View();
        }

        // POST: TicketsAuto1/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ticketID,originatorID,responsibleID,dateComposed,expiryDate,headerText,description,links,dateSubmitted,dateL1Release,dateL2Release,reason,adminComments,sanityCheck,summary,report")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.responsibleID = new SelectList(db.UserProfiles, "userID", "loginName", ticket.responsibleID);
            ViewBag.originatorID = new SelectList(db.UserProfiles, "userID", "loginName", ticket.originatorID);
            return View(ticket);
        }

        // GET: TicketsAuto1/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            ViewBag.responsibleID = new SelectList(db.UserProfiles, "userID", "loginName", ticket.responsibleID);
            ViewBag.originatorID = new SelectList(db.UserProfiles, "userID", "loginName", ticket.originatorID);
            return View(ticket);
        }

        // POST: TicketsAuto1/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ticketID,originatorID,responsibleID,dateComposed,expiryDate,headerText,description,links,dateSubmitted,dateL1Release,dateL2Release,reason,adminComments,sanityCheck,summary,report")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.responsibleID = new SelectList(db.UserProfiles, "userID", "loginName", ticket.responsibleID);
            ViewBag.originatorID = new SelectList(db.UserProfiles, "userID", "loginName", ticket.originatorID);
            return View(ticket);
        }

        // GET: TicketsAuto1/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: TicketsAuto1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
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
