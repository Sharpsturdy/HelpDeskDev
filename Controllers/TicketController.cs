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
        
        // GET: Ticket
        public ActionResult Index()
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
            UserData ud = new UserData();
            //UserProfile userProfile = ud.getUserProfile();

            Ticket ticket = new Ticket
            {
                //UserProfile = ud.getUserProfile(),
                //User originatorFullname = userProfile.displayName,
                //originatorID = ud.getUserProfile().userID,
                //status = Statuses.Draft,
                dateComposed = DateTime.Now
            };
            ViewBag.newTicket = "1";

            return View(ticket);
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
                                
                //ticket.headerText = tvm.headerText;
                //ticket.description = tvm.description;

                ticket.dateComposed = DateTime.Now;
                ticket.originatorID = userProfile.userID;
                ticket = db.Tickets.Add(ticket);
                //db.SaveChanges();
                /***** Add File ************/
                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase file = Request.Files[0];
                    Attachment attachment = new Attachment();
                    if (file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        attachment.filePath = Path.Combine(Server.MapPath("~/App_Data/Files"), fileName);
                        attachment.fileName = fileName;
                        attachment.parentID = ticket.ID;
                        file.SaveAs(attachment.filePath);
                    }
                    
                    db.Attachments.Add(attachment);
                    db.SaveChanges();
                    //return RedirectToAction("Index");
                }


                /****** Add File End ******/

                
                return RedirectToAction("Index");
            }

            return View(ticket);
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
            return View(ticket);
        }

        // POST: Ticket/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,originatorUsername,dateComposed,headerText,description,dateSubmitted,adminEmail,dateL1Release,dateL2Release,sanityCheck")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ticket);
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
