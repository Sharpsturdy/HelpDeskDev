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
    public class KnowledgeBaseController : Controller
    {
        private HelpDeskContext db = new HelpDeskContext();

        // GET: KB
        public ActionResult Index(int? page)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;

            return View(db.KnowledgeFAQs.Where(k => k.type == 2 && k.published)
                    .OrderByDescending(k => k.dateComposed)
                    .ToPagedList(currentPageIndex, AllSorts.pageSize));
            //return View(db.KnowledgeFAQs.Where(k => k.type == 2 && k.published).ToList());            
        }

        public ActionResult Admin(string searchType, string searchStr, int? page)
        {
            var kbs = from m in db.KnowledgeFAQs
                       where (m.type == 2)
                       select m;

            if (String.IsNullOrEmpty(searchType))
            {
                kbs = kbs.Where(s => !s.published);
            }
            else if (searchType == "1")
            {
                kbs = kbs.Where(s => s.expiryDate <= DateTime.Today);
            }
            else if (searchType == "2")
            {
                kbs = kbs.Where(s => s.published);
            }

            if (!String.IsNullOrEmpty(searchStr))
            {
                kbs = kbs.Where(s => s.headerText.Contains(searchStr) || s.description.Contains(searchStr));
            }

            //ViewBag.selectedOption = "" + searchType;
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;

            //ViewBag.selectedOption = ""+ searchType;
            return View("Index", kbs.OrderByDescending(m => m.dateComposed).ToPagedList(currentPageIndex, AllSorts.pageSize)); ;
            //return View("Index", kbs.ToList()); ;
        }

        public ActionResult Search(string searchStr, int? page)
        {
            var kbs = from m in db.KnowledgeFAQs
                       where (m.type == 2 && m.published)
                       select m;

            if (!String.IsNullOrEmpty(searchStr))
            {
                kbs = kbs.Where(s => s.headerText.Contains(searchStr) || s.description.Contains(searchStr));
            }


            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            return View("Index", kbs.OrderByDescending(m => m.dateComposed).ToPagedList(currentPageIndex, AllSorts.pageSize)); ;

        }

        // GET: KB/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KnowledgeFAQ kb = db.KnowledgeFAQs.Find(id);
            if (kb == null)
            {
                return HttpNotFound();
            }

            ViewBag.mode = 2;
            return View("KBOne", kb);
        }

        // GET: KB/New
        public ActionResult New()
        {
            //ViewBag.originatorID = new SelectList(db.UserProfiles, "userID", "loginName");
            ViewBag.mode = 0;
            return View("KBOne");
        }

        // POST: KB/New
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult New([Bind(Include = "type,headerText,description,links")] KnowledgeFAQ kb)
        {
            if (ModelState.IsValid)
            {
                UserData ud = new UserData();
                UserProfile userProfile = ud.getUserProfile();

                kb.dateComposed = DateTime.Now;
                kb.originatorID = userProfile.userID;

                if (Request.Form.AllKeys.Contains("btnApprove"))
                {
                    kb.published = true;
                    kb.expiryDate = kb.dateComposed.AddDays(AllSorts.getExpiryDays(db, true));

                    if (kb.dateSubmitted == null)
                        kb.dateSubmitted = DateTime.Now;

                }
                else if (Request.Form.AllKeys.Contains("btnUnApprove"))
                {
                    kb.published = false;
                }
                else if (Request.Form.AllKeys.Contains("btnSubmit"))
                {
                    kb.dateSubmitted = DateTime.Now;
                }

                kb = db.KnowledgeFAQs.Add(kb);
                db.SaveChanges(); 

                /***** Add Files ************/
                AllSorts.saveAttachments(kb.ID, db, null, 1);

                /***** Save keyowrds/expertareas *********/
                AllSorts.saveWordLists(Request.Form.GetValues("inkeywords"), Request.Form.GetValues("inexpertareas"), db, kb);

                db.SaveChanges();

                //Better to send mail post save in case there errors
                if (Request.Form.AllKeys.Contains("btnSubmit"))
                {

                    //Send email to ticket admins to let them know of this new ticket submission
                    Hangfire.BackgroundJob.Enqueue<Emailer>(x => x.sendFAQKBNotification("Submitted", kb.ID));
                }

                if (Request.Form.AllKeys.Contains("btnSave")) // || Request.Form.AllKeys.Contains("btnApprove") || Request.Form.AllKeys.Contains("btnUnApprove"))
                {
                    return RedirectToAction("Edit/" + kb.ID);
                }

                if (!string.IsNullOrEmpty((string)Session["lastView"]))
                    return Redirect((string)Session["lastView"]);

                return RedirectToAction("Index");
            }

            ViewBag.mode = 0;
            return View("KBOne", kb);
        }
        
        // GET: KB/Suggest
        public ActionResult Suggest()
        {
            return View();
        }

        // GET: KB/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KnowledgeFAQ kb = db.KnowledgeFAQs.Find(id);
            if (kb == null)
            {
                return HttpNotFound();
            }

            ViewBag.mode = 1;
            return View("KBOne", kb);
        }

        // POST: KB/Edit/5
        [HttpPost]
        public ActionResult Edit([Bind(Include = "ID,originatorID,expiryDate,dateComposed,type,headerText,description,links,deleteField,published")] KnowledgeFAQ kb)
        {
            if (ModelState.IsValid)
            {

                db.Entry(kb).State = EntityState.Modified;

                if (Request.Form.AllKeys.Contains("btnApprove"))
                {
                    kb.published = true;

                    //If being approved from expired then calculate expiry date from now instead of composed date
                    kb.expiryDate = (kb.status == Statuses.Expired ? DateTime.Now : kb.dateComposed).AddDays(AllSorts.getExpiryDays(db, true));                    
                }
                else if (Request.Form.AllKeys.Contains("btnUnApprove"))
                {
                    kb.published = false;
                }

                /***** Add File ************/
                AllSorts.saveAttachments(kb.ID, db, kb.deleteField, 1);

                /***** Save keyowrds/expertareas *********/
                AllSorts.saveWordLists(Request.Form.GetValues("inkeywords"), Request.Form.GetValues("inexpertareas"), db, kb);

                db.SaveChanges();

                //Better to send mail post save in case there errors
                if (Request.Form.AllKeys.Contains("btnSubmit"))
                {

                    //Send email to ticket admins to let them know of this new ticket submission
                    Hangfire.BackgroundJob.Enqueue<Emailer>(x => x.sendTicketNotification("Submitted",kb.ID));
                }
                else if (Request.Form.AllKeys.Contains("btnApprove"))
                {
                    //Send Email to originator to inform of approval
                    Hangfire.BackgroundJob.Enqueue<Emailer>(x => x.sendFAQKBNotification("Approved", kb.ID));

                }

                if (Request.Form.AllKeys.Contains("btnSave")) // || Request.Form.AllKeys.Contains("btnApprove") || Request.Form.AllKeys.Contains("btnUnApprove"))
                {
                    return RedirectToAction("Edit/" + kb.ID);
                }

                if (!string.IsNullOrEmpty((string)Session["lastView"]))
                    return Redirect((string)Session["lastView"]);

                return RedirectToAction("Index");
            }

            ViewBag.mode = 1;
            return View("KBOne", kb);
        }

        // GET: KB/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: KB/Delete/5
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
