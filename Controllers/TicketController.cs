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
            
            ViewBag.newTicket = "1";
            ViewBag.addAttachCode = "1"; //Activate attach code 
            ViewBag.addTinyMCECode = "1"; //Activate tinymce code

            return View();
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

                /***** Add File ************/
                //saveAttachments(ticket.ID);
                AllSorts.saveAttachments(ticket.ID, db);
                db.SaveChanges();

                //return RedirectToAction("Index");
                return RedirectToAction("Edit/" + ticket.ID);
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

            ViewBag.addAttachCode = "1"; //Activate attach code 
            ViewBag.addTinyMCECode = "1"; //Activate tinymce code

            return View(ticket);
        }

        // POST: Ticket/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,headerText,description,dateComposed, originatorID,links,deleteField")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                //Ticket ticket = db.Tickets.Find(ticketM.ID);
                db.Entry(ticket).State = EntityState.Modified;

                //ticket.description = ticketM.description;
                //ticket.headerText = ticketM.headerText;
                //ticket.links = ticketM.links;
                /***** Add File ************/
                //saveAttachments(ticket.ID, ticket.deleteField);
                AllSorts.saveAttachments(ticket.ID, db, ticket.deleteField);
                
                db.SaveChanges();
                //return RedirectToAction("Index");                
                return Redirect(Request.UrlReferrer.ToString());
            }

            ViewBag.addAttachCode = "1"; //Activate attach code 
            ViewBag.addTinyMCECode = "1"; //Activate tinymce code

            return View(ticket);
        }
        /*
        public ActionResult Edit([Bind(Include = "ID,headerText,description,links")] Ticket ticketM)
        {
            if (ModelState.IsValid)
            {
                Ticket ticket = db.Tickets.Find(ticketM.ID);
                db.Entry(ticket).State = EntityState.Modified;

                ticket.description = ticketM.description;
                ticket.headerText = ticketM.headerText;
                ticket.links = ticketM.links;
                /***** Add File ************ /
                saveAttachments(ticket.ID);

                db.SaveChanges();
                //return RedirectToAction("Index");
                return Redirect(Request.UrlReferrer.ToString());
            }
            return View(ticketM);
        }
        */
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

        private void saveAttachments(int ID, string deleteList = null)
        {
            if (Request.Files.Count > 0)
            {
                //Remove files
                if (deleteList != null)
                {
                    var fileIDs = deleteList.Split(new char[',']);

                    foreach(string strID in fileIDs)
                    {
                        if (!string.IsNullOrEmpty(strID))
                        {
                            Attachment f = db.Attachments.Find(int.Parse(strID));
                            db.Attachments.Remove(f);
                        }
                    }
                }

                //Add Files
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase file = Request.Files[i];
                    if (file.ContentLength > 0)
                    {

                        //Get physical path to directory
                        string savePath = Path.Combine(Server.MapPath("~/App_Data/Files"), DateTime.Now.Year.ToString());
                        if(!Directory.Exists(savePath)) {
                            Directory.CreateDirectory(savePath);
                        }

                        //Get unique random name, duplication not very possible
                        string randomName = ""; 
                        do {
                            randomName = Path.GetRandomFileName();
                        } while (System.IO.File.Exists(Path.Combine(savePath, randomName)));

                        //Save file
                        //file.SaveAs(savePath + "/" + randomName);
                        file.SaveAs(Path.Combine(savePath, randomName));

                        //Add file data to database
                        Attachment attachment = new Attachment();
                        attachment.fileName = Path.GetFileName(file.FileName);
                        attachment.filePath = "~/App_Data/Files/" + DateTime.Now.Year + "/" + randomName;
                        attachment.parentID = ID;     
                        db.Attachments.Add(attachment);
                    }
                }
                
            }
        }

    }
}
