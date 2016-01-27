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
using Help_Desk_2.Utilities;
using System.IO;
using MvcPaging;
using Hangfire;
using Help_Desk_2.BackgroundJobs;

namespace Help_Desk_2.Controllers
{
    public class TicketController : Controller
    {
        private HelpDeskContext db = new HelpDeskContext();

        //List all my tickets draft/open
        public ActionResult Index(string searchType, int? page)
        {

            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;     

            return View(db.Tickets.OrderByDescending(t => t.dateComposed).ToPagedList(currentPageIndex, AllSorts.pageSize));
        }

        //List all my tickets draft/open/closed
        public ActionResult List(int? page)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            return View("Index", db.Tickets.OrderByDescending(t => t.dateComposed).ToPagedList(currentPageIndex, AllSorts.pageSize));
        }

        //List all tickets from all user for administration
        public ActionResult Admin(int? page)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            return View("Index", db.Tickets.OrderByDescending(t => t.dateSubmitted).ToPagedList(currentPageIndex, AllSorts.pageSize));
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

            ViewBag.mode = 2;
            return View("TicketOne",ticket);
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
                    ticket.expiryDate = ticket.dateSubmitted.Value.AddDays(AllSorts.getExpiryDays(db));
                }
                
                ticket = db.Tickets.Add(ticket);

                /***** Add File ************/
                AllSorts.saveAttachments(ticket.ID, db);
                db.SaveChanges();

                //Better to send mail post save in case there errors
                if (Request.Form.AllKeys.Contains("btnSubmit"))
                {
                    Emailer em = new Emailer();
                    string email = "" + ticket.UserProfile.emailAddress;
                    int id = 0 + ticket.ID;
                    em.sendTicketNotification("SubmitTicket", email, id);
                }

                if (Request.Form.AllKeys.Contains("btnSave")) // || Request.Form.AllKeys.Contains("btnSubmit") || Request.Form.AllKeys.Contains("btnUnApprove"))
                {
                    return RedirectToAction("Edit/" + ticket.ID);
                }

                if (!string.IsNullOrEmpty((string)Session["lastView"]))
                    return Redirect((string)Session["lastView"]);

                return RedirectToAction("Index");
            }

            ViewBag.mode = 0;
            //ViewBag.responsibleID = new SelectList(db.UserProfiles, "userID", "displayName", ticket.responsibleID);
            ViewBag.errorMsg = "Model Binding failed";
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
            ViewBag.responsibleID = new SelectList(AllSorts.AllUsers.Where(x => x.isResponsible), "userID", "displayName", ticket.responsibleID);
            return View("TicketOne", ticket);
        }

        // POST: Ticket/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,headerText,description,dateComposed,dateSubmitted,dateL1Release,dateL2Release,originatorID,links,deleteField,"+ "sanityCheck,ticketID,adminComments,reason,responsibleID")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                //Ticket ticket = db.Tickets.Find(ticketM.ID);
                db.Entry(ticket).State = EntityState.Modified;

                if (Request.Form.AllKeys.Contains("btnSubmit"))
                {
                    ticket.dateSubmitted = DateTime.Now;
                    ticket.expiryDate = ticket.dateSubmitted.Value.AddDays(AllSorts.getExpiryDays(db));

                }
                /***** Add File ************/
                AllSorts.saveAttachments(ticket.ID, db, ticket.deleteField);
                
                if(ticket.sanityCheck == SanityChecks.Accept && ticket.dateL1Release == null)
                {
                    ticket.dateL1Release = DateTime.Now;
                }

                if(Request.Form.AllKeys.Contains("btnAssign"))
                {
                    ticket.dateL2Release = DateTime.Now;
                    //Send email to assigned
                }

                db.SaveChanges();

                //Better to send mail post save in case there errors
                if (Request.Form.AllKeys.Contains("btnSubmit"))
                {
                    //BackgroundJob.Enqueue(() => Mailer.sendNotification());
                    //BackgroundJob.Enqueue<Emailer>(x => x.sendTicketNotification("SubmitTicket", ticket.UserProfile.emailAddress, ticket.ID));
                    //Emailer em = new Emailer();
                    //em.sendTicketNotification("SubmitTicket", ticket.UserProfile.emailAddress, ticket.ID)
                    //Mailer.sendTicketNotification("SubmitTicket", ticket.UserProfile.emailAddress, ticket.ID);
                    // Hangfire.BackgroundJob.Enqueue<Emailer>(me => me.sendTicketNotification("SubmitTicket", ticket.UserProfile.emailAddress, ticket.ID));

                    Emailer em = new Emailer();
                    string email = ticket.UserProfile.emailAddress;
                    int id = ticket.ID;
                    em.sendTicketNotification("SubmitTicket", email + "", id + 0);

                    //Trying this
                    HelpDeskContext db = new HelpDeskContext();
                    Ticket t = db.Tickets.Find(18);
                    Hangfire.BackgroundJob.Enqueue<Emailer>(me => me.sendTicketNotification("SubmitTicket", t.UserProfile.emailAddress, 5000 + t.ID));

                }

                if (Request.Form.AllKeys.Contains("btnSave")) //|| Request.Form.AllKeys.Contains("btnSubmit") || Request.Form.AllKeys.Contains("btnUnApprove"))
                {
                    return RedirectToAction("Edit/" + ticket.ID);
                }

                if (!string.IsNullOrEmpty((string)Session["lastView"]))
                    return Redirect((string)Session["lastView"]);

                return RedirectToAction("Index");
            }

            ViewBag.mode = 1;
            ViewBag.responsibleID = new SelectList(AllSorts.AllUsers.Where(x => x.isResponsible), "userID", "displayName", ticket.responsibleID);

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
