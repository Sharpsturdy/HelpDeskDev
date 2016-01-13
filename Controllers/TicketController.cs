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

using System.Security.Principal;
using System.DirectoryServices.AccountManagement;
using Help_Desk_2.ViewModels;
using Help_Desk_2.Utilities;
using System.IO;

namespace Help_Desk_2.Controllers
{
    public class TicketController : Controller
    {
        private HelpDeskContext db = new HelpDeskContext();

        //List all my tickets draft/open
        public ActionResult Index()
        {

            return View(db.Tickets.ToList());
        }

        //List all my tickets draft/open/closed
        public ActionResult List()
        {
            return View(db.Tickets.ToList());
        }

        //List all tickets from all users draft/open/closed
        public ActionResult Admin()
        {
            return View(db.Tickets.ToList());
        }

        // GET: Ticket/Details/5
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

        // GET: Ticket/Create
        public ActionResult New()
        {

            ViewBag.mode = 0;
            return View("TicketOne");
        }

        // POST: Ticket/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult New([Bind(Include = "headerText,description")] Ticket ticket)
        //"ID,originatorUsername,dateComposed,headerText,description,dateSubmitted,adminEmail,dateL1Release,dateL2Release,sanityCheck")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                UserData ud = new UserData();
                UserProfile userProfile = ud.getUserProfile();
                ticket.originatorID = userProfile.userID;
                ticket.dateComposed = DateTime.Now;

                if (Request.Form.AllKeys.Contains("btnSubmit"))
                {
                    ticket.dateSubmitted = DateTime.Now;
                    ticket.expiryDate = ticket.dateSubmitted.Value.AddDays(AllSorts.getExpiryDays(db, true));

                }
                
                ticket.expiryDate = ticket.dateComposed.AddDays(AllSorts.getExpiryDays(db));
                ticket = db.Tickets.Add(ticket);

                /***** Add File ************/
                AllSorts.saveAttachments(ticket.ID, db);
                db.SaveChanges();

                if (Request.Form.AllKeys.Contains("btnSave") || Request.Form.AllKeys.Contains("btnSubmit") || Request.Form.AllKeys.Contains("btnUnApprove"))
                {
                    return RedirectToAction("Edit/" + ticket.ID);
                }

                if (!string.IsNullOrEmpty((string)Session["lastView"]))
                    return Redirect((string)Session["lastView"]);

                return RedirectToAction("Index");
            }

            ViewBag.mode = 0;
            return View("TicketOne", ticket);
        }

        // GET: Ticket/Edit/5
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

            
            ViewBag.mode = 1;
            return View("TicketOne", ticket);
        }

        // POST: Ticket/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,headerText,description,dateComposed,dateSubmitted,dateL1Release,dateL2Release,originatorID,links,deleteField")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                //Ticket ticket = db.Tickets.Find(ticketM.ID);
                db.Entry(ticket).State = EntityState.Modified;                
                
                /***** Add File ************/
                AllSorts.saveAttachments(ticket.ID, db, ticket.deleteField);
                
                db.SaveChanges();
                if (Request.Form.AllKeys.Contains("btnSave") || Request.Form.AllKeys.Contains("btnSubmit") || Request.Form.AllKeys.Contains("btnUnApprove"))
                {
                    return RedirectToAction("Edit/" + ticket.ID);
                }

                if (!string.IsNullOrEmpty((string)Session["lastView"]))
                    return Redirect((string)Session["lastView"]);

                return RedirectToAction("Index");
            }

            ViewBag.mode = 1;
            return View("TicketOne", ticket);
        }
       
                // GET: Ticket/Delete/5
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

        // POST: Ticket/Delete/5
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
