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
    public class FAQsController : Controller
    {
        private HelpDeskContext db = new HelpDeskContext();

        // GET: FAQs
        
        public ActionResult Index(int? page)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            
            return View(db.KnowledgeFAQs.Where(k => k.type == 1 && !k.suggest && k.published)
                    .OrderByDescending(k => k.dateComposed)
                    .ToPagedList(currentPageIndex, AllSorts.pageSize));
                   
        }

        public ActionResult Drafts(int? page)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;

            string userName = AllSorts.getUserID();
            return View("Index", db.KnowledgeFAQs.Where(k => k.type == 1 && k.dateSubmitted == null && (k.originatorID.ToString() == userName))
                    .OrderByDescending(k => k.dateComposed)
                    .ToPagedList(currentPageIndex, AllSorts.pageSize));
        }


        public ActionResult Admin(string searchType, string searchStr, int? page)
        {
            if (!AllSorts.UserCan("ManageFAQs"))
                return RedirectToAction("Unauthorized", "Home");

            var faqs = from m in db.KnowledgeFAQs
                       where (m.type == 1 && !m.suggest && m.dateSubmitted != null)
                       select m;

            if (String.IsNullOrEmpty(searchType))
            {
                faqs = faqs.Where(s => !s.published);
            }
            else if (searchType == "1")
            {
                faqs = faqs.Where(s => s.expiryDate <= DateTime.Today);
            }
            else if (searchType == "2")
            {
                faqs = faqs.Where(s => s.published);
            }

            if (!String.IsNullOrEmpty(searchStr))
            {
                faqs = faqs.Where(s => s.headerText.Contains(searchStr) || s.description.Contains(searchStr));
            }
            
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;

            //ViewBag.selectedOption = ""+ searchType;
            return View("Index", faqs.OrderByDescending(m=>m.dateComposed).ToPagedList(currentPageIndex, AllSorts.pageSize));           
        }

        public ActionResult Search(string searchStr, int? page)
        {
            var faqs = from m in db.KnowledgeFAQs
                       where(m.type == 1 && !m.suggest && m.published)
                       select m;

            if (!String.IsNullOrEmpty(searchStr))
            {
                faqs = faqs.Where(s => s.headerText.Contains(searchStr) || s.description.Contains(searchStr));
            }

            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            return View("Index",faqs.OrderByDescending(m => m.dateComposed).ToPagedList(currentPageIndex, AllSorts.pageSize));
        }

        // GET: FAQs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KnowledgeFAQ faq = db.KnowledgeFAQs.Find(id);
            if (faq == null)
            {
                return HttpNotFound();
            }

            ViewBag.mode = 2;
            return View(faq.suggest ? "Suggest" : "FAQOne", faq);
        }

        // GET: FAQs/New
        public ActionResult New()
        {
            //ViewBag.originatorID = new SelectList(db.UserProfiles, "userID", "loginName");
            ViewBag.mode = 0;
            return View("FAQOne");

        }

        // POST: FAQs/New
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult New([Bind(Include = "type,headerText,description,links")] KnowledgeFAQ faq)
        {
            if (ModelState.IsValid)
            {
                UserData ud = new UserData();
                UserProfile userProfile = ud.getUserProfile();
                var submittedValues = Request.Form.AllKeys;
                string outMsg = "New FAQ article created successfully";

                faq.dateComposed = DateTime.Now;
                faq.originatorID = userProfile.userID;

                if (submittedValues.Contains("btnApprove"))
                {

                    faq.published = true;
                    faq.expiryDate = faq.dateComposed.AddDays(AllSorts.getExpiryDays(1));

                    if (faq.dateSubmitted == null)
                        faq.dateSubmitted = DateTime.Now;

                    outMsg = "New FAQ created and approved successfully!";

                }
                else if (submittedValues.Contains("btnUnApprove"))
                {
                    faq.published = false;
                }

                faq = db.KnowledgeFAQs.Add(faq);

                db.SaveChanges();

                /***** Add File ************/
                AllSorts.saveAttachments(faq.ID, db, null, 1);

                /***** Save keyowrds/expertareas *********/
                AllSorts.saveWordLists(Request.Form.GetValues("inkeywords"), Request.Form.GetValues("inexpertareas"), db, faq);

                db.SaveChanges();
                //Better to send mail post save in case there errors
                if (submittedValues.Contains("btnSubmit"))
                {

                    //Send email to ticket admins to let them know of this new ticket submission
                    outMsg = "New FAQ created and submitted successfully!";
                    Hangfire.BackgroundJob.Enqueue<Emailer>(x => x.sendFAQKBNotification("Submitted",faq.ID));
                }

                AllSorts.displayMessage = outMsg;

                if (submittedValues.Contains("btnSave")) // || submittedValues.Contains("btnApprove") || submittedValues.Contains("btnUnApprove"))
                {
                    
                    return RedirectToAction("Edit/" + faq.ID);
                }

                if (!string.IsNullOrEmpty((string)Session["lastView"]))
                    return Redirect((string)Session["lastView"]);

                return RedirectToAction("Index");
            }
            else
            {
                AllSorts.displayMessage = "0#General error creating FAQ Article";
            }

            ViewBag.mode = 0;
            return View("FAQOne", faq);
        }
        
        // GET: FAQs/Suggest
        public ActionResult Suggest()
        {
            ViewBag.mode = 0;
            return View();
        }

        // POST: FAQs/Suggest
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Suggest([Bind(Include = "type,headerText,description")] KnowledgeFAQ faq)
        {
            if (ModelState.IsValid)
            {
                UserData ud = new UserData();
                UserProfile userProfile = ud.getUserProfile();

                faq.dateComposed = DateTime.Now;
                faq.originatorID = userProfile.userID;
                faq.suggest = true;
                
                faq = db.KnowledgeFAQs.Add(faq);
                db.SaveChanges();

                if (Request.Form.AllKeys.Contains("btnSave"))
                {
                    return RedirectToAction("Edit/" + faq.ID);
                }
                return RedirectToAction("Index");
            }

            ViewBag.mode = 0;
            return View(faq);
        }

        public ActionResult MySuggestions(int? page)
        {
            string userName = AllSorts.getUserID();
            var faqs = from m in db.KnowledgeFAQs
                       where (m.type == 1 && m.suggest)
                       select m;

            faqs = faqs.Where(s => s.originatorID.ToString() == userName);
            
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            return View("Index", faqs.OrderByDescending(m => m.dateComposed).ToPagedList(currentPageIndex, AllSorts.pageSize));

        }
        public ActionResult Suggestions(string searchType, string searchStr, int? page)
        {
            if (!AllSorts.UserCan("ManageFAQs"))
                return RedirectToAction("Unauthorized", "Home");

            var faqs = from m in db.KnowledgeFAQs
                       where (m.type == 1 && m.suggest)
                       select m;

            if (String.IsNullOrEmpty(searchType))
            {
                faqs = faqs.Where(s => !s.published);
            }
            else if (searchType == "1")
            {
                faqs = faqs.Where(s => s.expiryDate <= DateTime.Today);
            }
            else if (searchType == "2")
            {
                faqs = faqs.Where(s => s.published);
            }

            if (!String.IsNullOrEmpty(searchStr))
            {
                faqs = faqs.Where(s => s.headerText.Contains(searchStr) || s.description.Contains(searchStr));
            }

            //ViewBag.selectedOption = "" + searchType;

            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            return View("Index", faqs.OrderByDescending(m => m.dateComposed).ToPagedList(currentPageIndex, AllSorts.pageSize));

        }

        // GET: FAQs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KnowledgeFAQ faq = db.KnowledgeFAQs
                .Include(jkk => jkk.wordList)
                .Single(x => x.ID == (int)id);

            if (faq == null)
            {
                return HttpNotFound();
            }

            ViewBag.mode = 1;            
            return View(faq.suggest ? "Suggest":"FAQOne", faq);
        }

        // POST: FAQs/Edit/5
        [HttpPost]
        public ActionResult Edit([Bind(Include = "ID,originatorID,expiryDate,dateComposed,dateSubmitted,type,headerText,description,links,deleteField,suggest,published")] KnowledgeFAQ faq)
        {
            if (ModelState.IsValid)
            {

                db.Entry(faq).State = EntityState.Modified;
                var submittedValues = Request.Form.AllKeys;
                string outMsg = "FAQ article updated successfully";

                if (submittedValues.Contains("btnApprove"))
                {
                    faq.published = true;

                    //If being approved from expired then calculate expiry date from now instead of composed date
                    outMsg = "FAQ approved successfully!";
                    faq.expiryDate = (faq.status == Statuses.Expired ? DateTime.Now : faq.dateComposed).AddDays(AllSorts.getExpiryDays(1));
                   
                } else if(submittedValues.Contains("btnUnApprove"))
                {
                    faq.published = false;
                    outMsg = "FAQ unapproved successfully!";

                }
                else if(submittedValues.Contains("btnConvert"))
                {
                    faq.suggest = false;
                    outMsg = "FAQ Suggestion converted successfully!";

                }

                if (!faq.suggest)
                {
                    /***** Add File ************/
                    AllSorts.saveAttachments(faq.ID, db, faq.deleteField, 1);

                    /***** Save keyowrds/expertareas *********/
                    AllSorts.saveWordLists(Request.Form.GetValues("inkeywords"), Request.Form.GetValues("inexpertareas"), db, faq);

                    db.SaveChanges();
                }

                //Better to send mail post save in case there errors
                if (submittedValues.Contains("btnSubmit"))
                {

                    //Send email to ticket admins to let them know of this new ticket submission
                    Hangfire.BackgroundJob.Enqueue<Emailer>(x => x.sendFAQKBNotification("Submitted", faq.ID));
                    outMsg = "FAQ submitted successfully!";

                }
                else if (submittedValues.Contains("btnApprove"))
                {
                    //Send Email to originator to inform of approval
                    Hangfire.BackgroundJob.Enqueue<Emailer>(x => x.sendFAQKBNotification("Approved", faq.ID));

                }
                AllSorts.displayMessage = outMsg;

                if (submittedValues.Contains("btnSave")) //|| submittedValues.Contains("btnApprove") || submittedValues.Contains("btnUnApprove"))
                {
                   
                    return RedirectToAction("Edit/" + faq.ID);
                }

                if (!string.IsNullOrEmpty((string)Session["lastView"]))
                    return Redirect((string)Session["lastView"]);

                return RedirectToAction("Index");
                //return string.IsNullOrEmpty((string) Session["lastView"]) ? Redirect((string) Session["lastView"]): RedirectToAction("Index");
               
            }
            else
            {
                AllSorts.displayMessage = "0#General error updating FAQ Article";
            }

            ViewBag.mode = 1;
            return View(faq.suggest ? "Suggest" : "FAQOne", faq);
        }

        
    }
}
