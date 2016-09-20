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
using Help_Desk_2.Utilities;
using MvcPaging;

namespace Help_Desk_2.Controllers
{
    public class NewsController : Controller
    {
        private HelpDeskContext db = new HelpDeskContext();

        // GET: News
        public ActionResult Index(int? page)
        {
            var news = from row in db.News
                       where row.published & !row.deleted
                       orderby row.sticky descending, row.publishedDate descending
                       select row;
            
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;

            return View(news.ToPagedList(currentPageIndex, AllSorts.pageSize));

        }

        // GET: News/Article/5
        public ActionResult Article(int? id)
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

        // GET: News for Admins  
        //[CustomAuthorise(Roles = "AdminUsers")]
        public ActionResult Admin(string searchType, string searchStr, int? page)
        {
            if (!AllSorts.userHasRole("AdminUsers"))
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

            var news = from m in db.News
                       where !m.deleted
                       select m;

            if (String.IsNullOrEmpty(searchType))
            {
                news = news.Where(s => !s.published);
            }
            
            else if (searchType == "2")
            {
                news = news.Where(s => s.published);
            }
            
            if (!String.IsNullOrEmpty(searchStr))
            {
                news = news.Where(s => s.title.Contains(searchStr) || s.body.Contains(searchStr));
            }

            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;

            return View(news.OrderByDescending(x=> x.creationDate).ToPagedList(currentPageIndex, AllSorts.pageSize));
            
        }

        // GET: News/Details/5 for admins
        //[CustomAuthorise(Roles = "AdminUsers")]
        public ActionResult Details(int? id)
        {
            if (!AllSorts.userHasRole("AdminUsers"))
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            
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
        // [CustomAuthorise(Roles = "AdminUsers")]
        public ActionResult New()
        {
            if (!AllSorts.userHasRole("AdminUsers"))
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

            //Check new ticket message
            GlobalSettings gs = db.GlobalSettingss.FirstOrDefault<GlobalSettings>();

            if ((gs != null && gs.ID != null && gs.TicketHeaderEnabled))
            {
                AllSorts.displayMessage = gs.TicketHeader;
            }
                ViewBag.mode = 0;
            return View("NewsOne");
        }

        // POST: News/New (aka Create)
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[CustomAuthorise(Roles = "AdminUsers")]
        public ActionResult New([Bind(Include = "ID,title,body,sticky,published,publishedDate,creationDate")] News news)
        {
            if (!AllSorts.userHasRole("AdminUsers"))
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

            if (!AllSorts.userHasRole("AdminUsers"))
                return RedirectToAction("Unauthorized", "Home");

            if (ModelState.IsValid)
            {
                UserData ud = new UserData();
                UserProfile userProfile = ud.getUserProfile();

                news.creationDate = DateTime.Now;
                news.originatorID = userProfile.userID;

                if (news.published && news.publishedDate == null)
                {
                    news.publishedDate = DateTime.Now;
                }
                db.News.Add(news);
                db.SaveChanges();

                /***** Add File ************/
                AllSorts.saveAttachments(news.ID, db, null, 2);
                db.SaveChanges();

                AllSorts.displayMessage = "News article saved successfully!";
                if(Request.Form.AllKeys.Contains("btnSave"))
                {
                    return RedirectToAction("Edit/" + news.ID);
                }

                if (!string.IsNullOrEmpty((string)Session["lastView"]))
                    return Redirect((string)Session["lastView"]);

                return RedirectToAction("Admin");
            }

            ViewBag.mode = 0;
            return View("NewsOne", news);
        }

        // GET: News/Edit/5
        //[CustomAuthorise(Roles = "AdminUsers")]
        public ActionResult Edit(int? id)
        {
            if (!AllSorts.userHasRole("AdminUsers"))
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = db.News.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }

            ViewBag.mode = 1;
            return View("NewsOne", news);
        }

        // POST: News/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[CustomAuthorise(Roles = "AdminUsers")]
        public ActionResult Edit([Bind(Include = "ID,originatorID,title,body,sticky,published,publishedDate,creationDate,deleted,deleteField")] News news)
        {
            if (!AllSorts.userHasRole("AdminUsers"))
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

            if (ModelState.IsValid)
            {
                var submittedValues = Request.Form.AllKeys;
                
                db.Entry(news).State = EntityState.Modified;
                
                if (news.published && news.publishedDate == null) {
                    news.publishedDate = DateTime.Now;
                }

                if (submittedValues.Contains("btnDelete"))
                {
                    news.deleted = true;
                    AllSorts.displayMessage = "News article deleted successfully!";
                } else if (submittedValues.Contains("btnUnDelete"))
                {
                    news.deleted = false;
                    AllSorts.displayMessage = "News article undeleted successfully!";
                }
                else
                {
                    AllSorts.displayMessage = "News article updated successfully!";
                }

                /***** Add File ************/
                AllSorts.saveAttachments(news.ID, db, news.deleteField, 2);

                db.SaveChanges();

                if (submittedValues.Contains("btnSave"))
                {
                    //Do some Stuff for this Button
                    return RedirectToAction("Edit/" + news.ID);
                }
                if (!string.IsNullOrEmpty((string)Session["lastView"]))
                    return Redirect((string)Session["lastView"]);

                return RedirectToAction("Admin");
            } else
            {
                AllSorts.displayMessage = "0#General update error";
            }
            
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
