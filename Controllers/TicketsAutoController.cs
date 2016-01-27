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
    public class TicketsAutoController : Controller
    {
        private HelpDeskContext db = new HelpDeskContext();

        // GET: TicketsAuto
        public ActionResult Index()
        {
            var tickets = db.Tickets.Include(t => t.Originator);
            return View(tickets.ToList());
        }

        // GET: TicketsAuto/Details/5
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

        // GET: TicketsAuto/Create
        public ActionResult Create()
        {
            ViewBag.originatorID = new SelectList(db.UserProfiles, "userID", "loginName");
            return View();
        }

        // POST: TicketsAuto/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ticketID,originatorID,dateComposed,expiryDate,headerText,description,links,dateSubmitted,adminEmail,dateL1Release,dateL2Release,sanityCheck")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.originatorID = new SelectList(db.UserProfiles, "userID", "loginName", ticket.originatorID);
            return View(ticket);
        }

        // GET: TicketsAuto/Edit/5
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
            ViewBag.originatorID = new SelectList(db.UserProfiles, "userID", "loginName", ticket.originatorID);
            return View(ticket);
        }

        // POST: TicketsAuto/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ticketID,originatorID,dateComposed,expiryDate,headerText,description,links,dateSubmitted,adminEmail,dateL1Release,dateL2Release,sanityCheck")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.originatorID = new SelectList(db.UserProfiles, "userID", "loginName", ticket.originatorID);
            return View(ticket);
        }

        // GET: TicketsAuto/Delete/5
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

        // POST: TicketsAuto/Delete/5
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
