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
            return View(db.KnowledgeFAQs.Where(k => k.type == 1).ToList());
            //return View(db.KnowledgeFAQs.ToList());            
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
            return View("FAQOne", faq);
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

                faq = db.KnowledgeFAQs.Add(faq);

                /***** Add File ************/
                AllSorts.saveAttachments(faq.ID, db, null, 1);
                db.SaveChanges();

                if (Request.Form.AllKeys.Contains("btnSave"))
                {
                    return RedirectToAction("Edit/" + faq.ID);
                }
                return RedirectToAction("Index");
            }

            ViewBag.mode = 0;
            return View("FAQOne", faq);
        }
        
        // GET: FAQs/Suggest
        public ActionResult Suggest()
        {
            return View();
        }

        // GET: FAQs/Edit/5
        public ActionResult Edit(int? id)
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

            ViewBag.mode = 1;
            return View("FAQOne", faq);
        }

        // POST: FAQs/Edit/5
        [HttpPost]
        public ActionResult Edit([Bind(Include = "ID,originatorID,expiryDate,dateComposed,type,headerText,description,links,deleteField")] KnowledgeFAQ faq)
        {
            if (ModelState.IsValid)
            {

                db.Entry(faq).State = EntityState.Modified;

                /***** Add File ************/
                AllSorts.saveAttachments(faq.ID, db, faq.deleteField, 1);
                db.SaveChanges();

                if (Request.Form.AllKeys.Contains("btnSave"))
                {
                    return RedirectToAction("Edit/" + faq.ID);
                }
                return RedirectToAction("Index");
            }

            ViewBag.mode = 1;
            return View("FAQOne", faq);
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
