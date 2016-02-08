using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Help_Desk_2.DataAccessLayer;
using Help_Desk_2.Models;
using Help_Desk_2.Utilities;
using MvcPaging;
using Help_Desk_2.BackgroundJobs;

namespace Help_Desk_2.Controllers
{
    public class TicketController : Controller
    {
        private HelpDeskContext db = new HelpDeskContext();

        //List all my tickets draft/open
        public ActionResult Index(int? page)
        {

            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;

            string userName = AllSorts.getUserID();
            var tickets = from t in db.Tickets
                          where (!t.deleted && t.originatorID.ToString() == userName && t.dateCompleted == null)
                          orderby t.dateComposed descending
                          select t;

            return View(tickets.ToPagedList(currentPageIndex, AllSorts.pageSize));
        }

        //List all my tickets draft/open/closed
        public ActionResult List(int? page)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;

            string userName = AllSorts.getUserID();
            var tickets = from t in db.Tickets
                          where (!t.deleted && (t.originatorID.ToString() == userName))
                          orderby t.dateComposed descending
                          select t;

            return View("Index", tickets.ToPagedList(currentPageIndex, AllSorts.pageSize));
        }

        //List all tickets from all user for administration
        //[CustomAuthorise(Roles = "ManageTickets")]
        public ActionResult Admin(string searchType, string searchStr, int? page)
        {
            if(!AllSorts.UserCan("ManageTickets")) 
                return RedirectToAction("Unauthorized", "Home");

            var tickets = from t in db.Tickets
                          where !t.deleted && t.dateSubmitted != null

                          select t;

            /* 
            new SelectListItem {Value = "", Text = "Open" },
        new SelectListItem {Value = "1", Text = "Submitted" },
        new SelectListItem {Value = "2", Text = "Checked" },
        new SelectListItem {Value = "3", Text = "Assigned" },
        new SelectListItem {Value = "4", Text = "Returned" },
        new SelectListItem {Value = "5", Text = "OnHold" },
        new SelectListItem {Value = "6", Text = "Completed" },
        new SelectListItem {Value = "7", Text = "All" }
            */
            switch (searchType)
            {
                case "":
                    tickets = tickets.Where(s => s.dateCompleted != null);
                    break;
                case "1":
                    tickets = tickets.Where(s => s.status == Statuses.Submitted);
                    break;
                case "2":
                    tickets = tickets.Where(s => s.status == Statuses.Checked);
                    break;
                case "3":
                    tickets = tickets.Where(s => s.status == Statuses.Assigned);
                    break;
                case "4":
                    tickets = tickets.Where(s => s.status == Statuses.Returned);
                    break;
                case "5":
                    tickets = tickets.Where(s => s.status == Statuses.OnHold);
                    break;
                case "6":
                    tickets = tickets.Where(s => s.status == Statuses.Completed);
                    break;
                /*case "7":
                    tickets = tickets.Where(s => s.status == Statuses.Submitted);
                    break;
                */
            }

            if (!String.IsNullOrEmpty(searchStr))
            {
                tickets = tickets.Where(s => s.headerText.Contains(searchStr) || s.description.Contains(searchStr));
            }

            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            return View("Index", tickets.OrderByDescending(t => t.dateSubmitted).ToPagedList(currentPageIndex, AllSorts.pageSize));
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

                ticket.originatorID = new Guid(AllSorts.getUserID());
                ticket.dateComposed = DateTime.Now;
                string auditMsg = "";
                var submittedValues = Request.Form.AllKeys;

                if (submittedValues.Contains("btnSubmit"))
                {
                    ticket.dateSubmitted = DateTime.Now;
                    ticket.expiryDate = ticket.dateSubmitted.Value.AddDays(AllSorts.getExpiryDays());                    
                }
                
                ticket = db.Tickets.Add(ticket);

                /***** Add File ************/
                AllSorts.saveAttachments(ticket.ID, db);
                db.SaveChanges();

                //Better to send mail post save in case there errors
                if (submittedValues.Contains("btnSubmit"))
                {

                    //Send email to ticket admins to let them know of this new ticket submission
                    Hangfire.BackgroundJob.Enqueue<Emailer>(x => x.sendTicketNotification("Submitted", ticket.ID));
                    auditMsg = "Ticket submitted and notification(s) sent to "
                        + string.Join(", ", AllSorts.AllUsers.Where(x => x.isResponsible).Select(x => x.displayName).ToArray());
                }
                else if (submittedValues.Contains("btnSave") || submittedValues.Contains("btnSClose"))
                {
                    auditMsg = "Ticket created";
                }
                
                if (auditMsg == "")
                    auditMsg = "Ticket modified";

                //Add action to audit trail
                AllSorts.addAuditTrail(db, ticket.ID, new Guid(AllSorts.getUserID()), auditMsg);
                db.SaveChanges();

                if(submittedValues.Contains("btnSave"))
                    return RedirectToAction("Edit/" + ticket.ID);
                else if (!string.IsNullOrEmpty((string)Session["lastView"]))
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

            //Check access
            bool allowAccess = false;
            if (ticket.dateSubmitted == null)
            {
                if (ticket.originatorID.ToString() == AllSorts.getUserID())
                {
                    allowAccess = true;
                }
            }
            else {
                if (ticket.responsibleID.ToString() == AllSorts.getUserID() || AllSorts.UserCan("ManageTickets"))
                {
                    allowAccess = true; // go ahead
                }
            }

            if(!allowAccess)
                return RedirectToAction("Unauthorized", "Home");

            ViewBag.mode = 1;
            ViewBag.responsibleID = new SelectList(AllSorts.AllUsers, "userID", "displayName", ticket.responsibleID);
            return View("TicketOne", ticket);
        }

        // POST: Ticket/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,headerText,description,dateComposed,dateSubmitted,dateL1Release,dateL2Release,originatorID,links,deleteField,"+ "sanityCheck,ticketID,adminComments,reason,responsibleID,dateCompleted")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                string auditMsg = "";
                var submittedValues = Request.Form.AllKeys;

                if (submittedValues.Contains("btnDelete") && ticket.dateSubmitted == null)
                {  //this is a draft so fully delete
                    Ticket tDel = db.Tickets.Find(ticket.ID);
                    db.Tickets.Remove(tDel);
                    db.SaveChanges();

                    //Quit. You're done!
                    if (!string.IsNullOrEmpty((string)Session["lastView"]))
                        return Redirect((string)Session["lastView"]);

                    return RedirectToAction("Index", "Home");                   
                }

                //Ticket ticket = db.Tickets.Find(ticketM.ID);
                db.Entry(ticket).State = EntityState.Modified;               

                /***** Add File ************/
                AllSorts.saveAttachments(ticket.ID, db, ticket.deleteField);

                if (ticket.sanityCheck != null && ticket.dateL1Release == null)
                {
                    ticket.dateL1Release = DateTime.Now;
                }

                //Button specific code
                if (submittedValues.Contains("btnDelete")) {
                    ticket.deleted = true;
                    auditMsg = "Ticket deleted";
                }
                else if (submittedValues.Contains("btnSubmit"))
                {
                    ticket.dateSubmitted = DateTime.Now;
                    ticket.expiryDate = ticket.dateSubmitted.Value.AddDays(AllSorts.getExpiryDays());

                    //Send email to ticket admins to let them know of this new ticket submission
                    Hangfire.BackgroundJob.Enqueue<Emailer>(x => x.sendTicketNotification("Submitted", ticket.ID));
                    auditMsg = "Ticket submitted and notification(s) sent to "
                        + string.Join(", ", AllSorts.AllUsers.Where(x => x.isResponsible).Select(x => x.displayName).ToArray());

                }
                
                else if(submittedValues.Contains("btnAssign"))
                {
                    if (ticket.dateL2Release == null)
                        ticket.dateL2Release = DateTime.Now;

                    //Assign ticket number from globals settings
                    ticket.ticketID = AllSorts.getNextTicketNumber();
                   
                    //Send email to assigned to let them know of this new ticket assignment
                    Hangfire.BackgroundJob.Enqueue<Emailer>(x => x.sendTicketNotification("Assigned", ticket.ID));
                    //auditMsg = "Ticket assigned and notification sent to "  + ticket.Responsible.displayName;
                }
                else if (submittedValues.Contains("btnReturn"))
                {
                    ticket.returned = true;
                    Hangfire.BackgroundJob.Enqueue<Emailer>(x => x.sendTicketNotification("Returned", ticket.ID));
                    auditMsg = "Ticket returned to queue and notification(s) sent to "
                        + string.Join(", ", AllSorts.AllUsers.Where(x => x.isResponsible).Select(x => x.displayName).ToArray()); 
                }
                else if (submittedValues.Contains("btnComplete"))
                {
                    ticket.dateCompleted = DateTime.Now;

                    //Send email to originator to let them know ticket is completed
                    Hangfire.BackgroundJob.Enqueue<Emailer>(x => x.sendTicketNotification("Completed", ticket.ID));
                    auditMsg = "Ticket assigned and notification sent to " + ticket.Responsible.displayName;
                }

                if (auditMsg == "")
                    auditMsg = "Ticket modified";

                //Add action to audit trail
                AllSorts.addAuditTrail(db, ticket.ID, new Guid(AllSorts.getUserID()), auditMsg);

                db.SaveChanges();
                                
                if (submittedValues.Contains("btnSave")) //|| submittedValues.Contains("btnSubmit") || submittedValues.Contains("btnUnApprove"))
                {
                    return RedirectToAction("Edit/" + ticket.ID);
                }

                if (!string.IsNullOrEmpty((string)Session["lastView"]))
                    return Redirect((string)Session["lastView"]);

                return RedirectToAction("Index");
            }

            ViewBag.mode = 1;
            ViewBag.responsibleID = new SelectList(AllSorts.AllUsers, "userID", "displayName", ticket.responsibleID);

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
