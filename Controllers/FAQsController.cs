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

namespace Help_Desk_2.Controllers
{
    public class FAQsController : Controller
    {
        private HelpDeskContext db = new HelpDeskContext();

        // GET: FAQs
        public ActionResult Index()
        {
            return View(db.KnowledgeFAQs.Where(k => k.type == 1 && !k.suggest && k.published).ToList());
            //return View(db.KnowledgeFAQs.ToList());            
        }

        public ActionResult Admin(string searchType, string searchStr)
        {
            var faqs = from m in db.KnowledgeFAQs
                       where (m.type == 1 && !m.suggest)
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

            ViewBag.selectedOption = ""+ searchType;
            return View("Index", faqs.ToList());;            
        }

        public ActionResult Search(string searchStr)
        {
            var faqs = from m in db.KnowledgeFAQs
                       where(m.type == 1 && !m.suggest && m.published)
                       select m;

            if (!String.IsNullOrEmpty(searchStr))
            {
                faqs = faqs.Where(s => s.headerText.Contains(searchStr) || s.description.Contains(searchStr));
            }

            ViewBag.displayMessage = searchStr;

            return View("Index",faqs.ToList());
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

                faq.dateComposed = DateTime.Now;
                faq.originatorID = userProfile.userID;

                if (Request.Form.AllKeys.Contains("btnApprove"))
                {
                    faq.published = true;
                    faq.expiryDate = faq.dateComposed.AddDays(AllSorts.getExpiryDays(db, true));

                }
                else if (Request.Form.AllKeys.Contains("btnUnApprove"))
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

                if (Request.Form.AllKeys.Contains("btnSave") || Request.Form.AllKeys.Contains("btnApprove") || Request.Form.AllKeys.Contains("btnUnApprove"))
                {
                    return RedirectToAction("Edit/" + faq.ID);
                }

                if (!string.IsNullOrEmpty((string)Session["lastView"]))
                    return Redirect((string)Session["lastView"]);

                return RedirectToAction("Index");
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

        public ActionResult Suggestions(string searchType, string searchStr)
        {
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

            ViewBag.selectedOption = "" + searchType;
            return View("Index", faqs.ToList()); ;
            //return View("Index", db.KnowledgeFAQs.Where(k => k.type == 1 && k.suggest).ToList());
                        
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
        public ActionResult Edit([Bind(Include = "ID,originatorID,expiryDate,dateComposed,type,headerText,description,links,deleteField,suggest,published")] KnowledgeFAQ faq)
        {
            if (ModelState.IsValid)
            {

                db.Entry(faq).State = EntityState.Modified;

                if (Request.Form.AllKeys.Contains("btnApprove"))
                {
                    faq.published = true;
                    faq.expiryDate = faq.dateComposed.AddDays(AllSorts.getExpiryDays(db, true));

                } else if(Request.Form.AllKeys.Contains("btnUnApprove"))
                {
                    faq.published = false;
                } else if(Request.Form.AllKeys.Contains("btnConvert"))
                {
                    faq.suggest = false;
                }

                if (!faq.suggest)
                {
                    /***** Add File ************/
                    AllSorts.saveAttachments(faq.ID, db, faq.deleteField, 1);

                    /***** Save keyowrds/expertareas *********/
                    AllSorts.saveWordLists(Request.Form.GetValues("inkeywords"), Request.Form.GetValues("inexpertareas"), db, faq);

                    db.SaveChanges();
                }

                if (Request.Form.AllKeys.Contains("btnSave") || Request.Form.AllKeys.Contains("btnApprove") || Request.Form.AllKeys.Contains("btnUnApprove"))
                {
                    return RedirectToAction("Edit/" + faq.ID);
                }

                if (!string.IsNullOrEmpty((string)Session["lastView"]))
                    return Redirect((string)Session["lastView"]);

                return RedirectToAction("Index");
                //return string.IsNullOrEmpty((string) Session["lastView"]) ? Redirect((string) Session["lastView"]): RedirectToAction("Index");
               
            }

            ViewBag.mode = 1;
            return View(faq.suggest ? "Suggest" : "FAQOne", faq);
        }

        // GET: FAQs/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: FAQs/Delete/5
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
