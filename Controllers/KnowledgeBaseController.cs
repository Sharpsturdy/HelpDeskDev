﻿using Help_Desk_2.DataAccessLayer;
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
    public class KnowledgeBaseController : Controller
    {
        private HelpDeskContext db = new HelpDeskContext();

        // GET: KB
        public ActionResult Index()
        {
            return View(db.KnowledgeFAQs.ToList());            
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

                kb = db.KnowledgeFAQs.Add(kb);

                /***** Add File ************/
                AllSorts.saveAttachments(kb.ID, db, null, 1);
                db.SaveChanges();

                if (Request.Form.AllKeys.Contains("btnSave"))
                {
                    return RedirectToAction("Edit/" + kb.ID);
                }
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
        public ActionResult Edit([Bind(Include = "ID,originatorID,expiryDate,dateComposed,type,headerText,description,links,deleteField")] KnowledgeFAQ kb)
        {
            if (ModelState.IsValid)
            {

                db.Entry(kb).State = EntityState.Modified;

                /***** Add File ************/
                AllSorts.saveAttachments(kb.ID, db, kb.deleteField, 1);
                db.SaveChanges();

                if (Request.Form.AllKeys.Contains("btnSave"))
                {
                    return RedirectToAction("Edit/" + kb.ID);
                }
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
