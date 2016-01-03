﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Help_Desk_2.DataAccessLayer;
using Help_Desk_2.Models;
using Help_Desk_2.Utilities;

namespace Help_Desk_2.Controllers
{
    public class NewsController : Controller
    {
        private HelpDeskContext db = new HelpDeskContext();

        // GET: News
        public ActionResult Index()
        {
            return View(db.News.ToList());
        }

        // GET: News/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = db.News.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            ViewBag.mode = 2;
            return View("NewsOne",news);
        }

        // GET: News/New (aka Create)
        public ActionResult New()
        {
            //ViewBag.addAttachCode = "1"; //Activate attach code 
            ViewBag.addTinyMCECode = "1"; //Activate tinymce code
            ViewBag.mode = 0;
            return View("NewsOne");
        }

        // POST: News/New (aka Create)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult New([Bind(Include = "ID,title,body,sticky,published,publishedDate,creationDate")] News news)
        {
            if (ModelState.IsValid)
            {
                UserData ud = new UserData();
                UserProfile userProfile = ud.getUserProfile();

                news.creationDate = DateTime.Now;
                news.originatorID = userProfile.userID;
                db.News.Add(news);
                db.SaveChanges();

                if(Request.Form.AllKeys.Contains("btnSave"))
                {
                    return RedirectToAction("Edit/" + news.ID);
                }
                return RedirectToAction("Index");
            }

            ViewBag.addTinyMCECode = "1"; //Activate tinymce code
            ViewBag.mode = 0;
            return View("NewsOne", news);
        }

        // GET: News/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = db.News.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }

            //ViewBag.addAttachCode = "1"; //Activate attach code 
            ViewBag.addTinyMCECode = "1"; //Activate tinymce code
            ViewBag.mode = 1;
            return View("NewsOne", news);
        }

        // POST: News/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,originatorID,title,body,sticky,published,publishedDate,creationDate")] News news)
        {
            if (ModelState.IsValid)
            {
                db.Entry(news).State = EntityState.Modified;
                db.SaveChanges();

                if (Request.Form.AllKeys.Contains("btnSave"))
                {
                    //Do some Stuff for this Button
                    return RedirectToAction("Edit/" + news.ID);
                }
                return RedirectToAction("Index");
            }

            
            //ViewBag.addAttachCode = "1"; //Activate attach code 
            ViewBag.addTinyMCECode = "1"; //Activate tinymce code
            ViewBag.mode = 1;
            return View("NewsOne", news);
        }

        /***
        // GET: News/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = db.News.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }


            return View(news);
        }

        // POST: News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            News news = db.News.Find(id);
            db.News.Remove(news);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        **/
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
