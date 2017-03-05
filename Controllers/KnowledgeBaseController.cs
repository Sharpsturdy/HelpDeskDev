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
using System.Web.Security;

namespace Help_Desk_2.Controllers
{
	
	[CustomAuthorise(Roles=UserRoles.DomainAdminAndSuperUserRoles)]
    public class KnowledgeBaseController : Controller
    {
        private HelpDeskContext db = new HelpDeskContext();

        // GET: KB
        public ActionResult Index(int? page)
        {
           int currentPageIndex = page.HasValue ? page.Value - 1 : 0;

            return View(db.KnowledgeFAQs.Where(k => k.type == 2 && k.published && !(k.deleted || k.archived))
                    .OrderByDescending(k => k.dateComposed)
                    .ToPagedList(currentPageIndex, AllSorts.pageSize));         
        }

        public ActionResult Drafts(int? page)
        {
           int currentPageIndex = page.HasValue ? page.Value - 1 : 0;

            string userName = AllSorts.getUserID();
            return View("Index",db.KnowledgeFAQs.Where(k => k.type == 2 && !k.deleted && k.dateSubmitted == null && (k.originatorID.ToString() == userName))
                    .OrderByDescending(k => k.dateComposed)
                    .ToPagedList(currentPageIndex, AllSorts.pageSize));            
        }

        public ActionResult Admin(string searchType, string Keywords, string ExpertAreas, string searchStr, int? page)
        {
                
            List<WordList> list = db.WordLists.Where(k => k.type == 1 && !k.deleted).ToList();
            ViewBag.Keywords = new SelectList(list, "text", "text");

            List<WordList> list2 = db.WordLists.Where(k => k.type == 2 && !k.deleted).ToList();
            ViewBag.ExpertAreas = new SelectList(list2, "text", "text");
            

            var kbs = from m in db.KnowledgeFAQs
                       where (m.type == 2 && !m.deleted && m.dateSubmitted != null)
                       select m;

            if (String.IsNullOrEmpty(searchType))
            {
                kbs = kbs.Where(s => !s.published);
            }
            /*else if (searchType == "1")
            {
                kbs = kbs.Where(s => s.expiryDate <= DateTime.Today);
            }*/
            else if (searchType == "2")
            {
                kbs = kbs.Where(s => s.published);
            }

            if (!String.IsNullOrEmpty(searchStr))
            {
                kbs = kbs.Where(s => s.headerText.Contains(searchStr) || s.description.Contains(searchStr));
            }

            if (!String.IsNullOrEmpty(Keywords))
            {
               Keywords.Replace("+", " ");               

                IEnumerable<WordList> keywordtext = db.WordLists.Where(m => m.type == 1 && !m.deleted && m.text == Keywords);
                
                kbs = from s in kbs where(s.wordList.Where(x => x.type == 1 && !x.deleted).Contains(keywordtext.FirstOrDefault())) select s;
               
                
            }
            if (!String.IsNullOrEmpty(ExpertAreas))
            {
                ExpertAreas.Replace("+", " ");                

                IEnumerable<WordList> expertareatext = db.WordLists.Where(m => m.type == 2 && !m.deleted && m.text == ExpertAreas);

                kbs = from s in kbs where (s.wordList.Where(x => x.type == 2 && !x.deleted).Contains(expertareatext.FirstOrDefault())) select s;


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
                       where (m.type == 2 && !m.deleted && m.published)
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

            //Is there archive article           
            if (kb.archiveID > 0)
            {
                KnowledgeFAQ kba = db.KnowledgeFAQs.Find(kb.archiveID);
                if (kba != null) ViewBag.archiveTitle = kba.headerText;
            }

            ViewBag.mode = 2;
            return View("KBOne", kb);
        }

        // GET: KB/New
        public ActionResult New(int? id)
        {
            ViewBag.mode = 0;
            
            if (id == null)
            {
                return View("KBOne");
            }
            else
            {
                KnowledgeFAQ kb = db.KnowledgeFAQs.Find(id);
                if (kb == null)
                {
                    return HttpNotFound();
                }
                kb.archiveID = (int) id;
                return View("KBOne", kb);
            }
            
        }

        // POST: KB/New
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult New([Bind(Include = "type,headerText,description,links,archiveID,notes")] KnowledgeFAQ kb)
        {
           if (ModelState.IsValid)
            {
                var submittedValues = Request.Form.AllKeys;

                kb.dateComposed = DateTime.Now;
                kb.originatorID = new Guid(AllSorts.getUserID());

                string outMsg = "New KB Article created successfully";

                if (submittedValues.Contains("btnApprove"))
                {
                    kb.published = true;
                    //@modifed 17/06/2016 KBs never expire 
                    //reinstated 23/11/2016
                    kb.expiryDate = kb.dateComposed.AddDays(AllSorts.getExpiryDays(2));

                    if (kb.dateSubmitted == null)
                        kb.dateSubmitted = DateTime.Now;

                    outMsg =  "New KB Article created and approved successfully";

                }
                else if (submittedValues.Contains("btnUnApprove"))
                {
                    kb.published = false;
                }
                else if (submittedValues.Contains("btnSubmit"))
                {
                    kb.dateSubmitted = DateTime.Now;
                    outMsg = "New KB Article created and submitted successfully";
                }

                kb = db.KnowledgeFAQs.Add(kb);
                db.SaveChanges(); 

                /***** Add Files ************/
                AllSorts.saveAttachments(kb.ID, db, null, 1);

                /***** Save keyowrds/expertareas *********/
                AllSorts.saveWordLists(Request.Form.GetValues("inkeywords"), Request.Form.GetValues("inexpertareas"), db, kb);

                db.SaveChanges();

                //Better to send mail post save in case there errors
                if (submittedValues.Contains("btnSubmit"))
                {

                    //Send email to ticket admins to let them know of this new ticket submission
                    Hangfire.BackgroundJob.Enqueue<Emailer>(x => x.sendFAQKBNotification("Submitted", kb.ID));
                }

                AllSorts.displayMessage = outMsg;
                if (submittedValues.Contains("btnSave")) // || submittedValues.Contains("btnApprove") || submittedValues.Contains("btnUnApprove"))
                {
                    return RedirectToAction("Edit/" + kb.ID);
                }

                if (!string.IsNullOrEmpty((string)Session["lastView"]))
                    return Redirect((string)Session["lastView"]);

                return RedirectToAction("KnowledgeBase/Index");
            } else
            {
                AllSorts.displayMessage = "0#General error creating KB Article";
                
            }

            ViewBag.mode = 0;
            return View("KBOne", kb);
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
            if (kb.archiveID > 0)
            {
                KnowledgeFAQ kba = db.KnowledgeFAQs.Find(kb.archiveID);
                if (kba != null) ViewBag.archiveTitle = kba.headerText;
            }
            return View("KBOne", kb);
        }

        // POST: KB/Edit/5
        [HttpPost]
        public ActionResult Edit([Bind(Include = "ID,originatorID,expiryDate,dateComposed,dateSubmitted,type,headerText,description,links,deleteField,published,archiveID,notes")] KnowledgeFAQ kb)
        {
            if (ModelState.IsValid)
            {
                var submittedValues = Request.Form.AllKeys;
                db.Entry(kb).State = EntityState.Modified;
                string outMsg = "KB Article updated successfully";

                if (submittedValues.Contains("btnApprove"))
                {
                    kb.published = true;

                    //If being approved from expired then calculate expiry date from now instead of composed date
                    //@modifed 17/06/2016 KBs never expire 
                    // reinstated 23/11/2016
                    kb.expiryDate = (kb.status == Statuses.Expired ? DateTime.Now : kb.dateComposed).AddDays(AllSorts.getExpiryDays(2));

                    outMsg = "KB Article approved successfully";
                }
                else if (submittedValues.Contains("btnUnApprove"))
                {
                    kb.published = false;
                    outMsg = "KB Article unapproved successfully";
                }
                else if (submittedValues.Contains("btnSubmit"))
                {
                    kb.dateSubmitted = DateTime.Now;
                    outMsg = "KB Article submitted successfully";
                }
                /***** Add File ************/
                AllSorts.saveAttachments(kb.ID, db, kb.deleteField, 1);

                /***** Save keyowrds/expertareas *********/
                AllSorts.saveWordLists(Request.Form.GetValues("inkeywords"), Request.Form.GetValues("inexpertareas"), db, kb);

                db.SaveChanges();

                //Better to send mail post save in case there errors
                if (submittedValues.Contains("btnSubmit"))
                {
                    //Send email to ticket admins to let them know of this new ticket submission
                    Hangfire.BackgroundJob.Enqueue<Emailer>(x => x.sendTicketNotification("Submitted",kb.ID));
                    
                }
                else if (submittedValues.Contains("btnApprove"))
                {
                    //Send Email to originator to inform of approval
                    Hangfire.BackgroundJob.Enqueue<Emailer>(x => x.sendFAQKBNotification("Approved", kb.ID));

                }

                AllSorts.displayMessage = outMsg;
                if (submittedValues.Contains("btnSave")) // || submittedValues.Contains("btnApprove") || submittedValues.Contains("btnUnApprove"))
                {
                    return RedirectToAction("Edit/" + kb.ID);
                }

                if (!string.IsNullOrEmpty((string)Session["lastView"]))
                    return Redirect((string)Session["lastView"]);

                return RedirectToAction("Index");
            }
            else
            {
                AllSorts.displayMessage = "0#General error updating KB Article";
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
                KnowledgeFAQ article = db.KnowledgeFAQs.Find(id);
                if (article != null)
                {
                    article.deleted = true;
                    db.SaveChanges();
                    return RedirectToAction("Admin");
                }
                return RedirectToAction("Edit", new { id = id });
            }
            catch
            {
                return View();
            }
        }
    }
}
