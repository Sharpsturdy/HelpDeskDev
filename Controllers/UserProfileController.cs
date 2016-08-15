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
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;
using Help_Desk_2.Utilities;
using MvcPaging;

namespace Help_Desk_2.Controllers
{
    public class UserProfileController : Controller
    {
        private HelpDeskContext db = new HelpDeskContext();

        // GET: UserProfile
        public ActionResult List(int? page)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;

            var lastGoodDate = DateTime.Now.AddDays(-30);
            return View(db.UserProfiles.Where(u => !u.deleted && u.lastSignOn < lastGoodDate)
                    .OrderByDescending(k => k.lastSignOn)
                    .OrderBy(k => k.surName)
                    .OrderBy(k => k.firstName)
                    .ToPagedList(currentPageIndex, AllSorts.pageSize));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List([Bind(Include = "userID")] UserProfile user)
        {
            if (ModelState.IsValid)
            {
                UserProfile u = db.UserProfiles.Find(user.userID);
                if (u != null)
                {
                    //db.UserProfiles.Remove(u); Problem with hard delete if attached to other records so do soft delete
                    db.Entry(u).State = EntityState.Modified;
                    u.deleted = true;
                    db.SaveChanges();

                    AllSorts.displayMessage = "User profile for '" + u.displayName +"' has been deleted successfully";
                }
            }
            return RedirectToAction("List");
        }

        // GET: UserProfile/Create
        public ActionResult Index()
        {
  
            UserProfile userProfile = db.UserProfiles.Find(new Guid(AllSorts.getUserID()));

            return View(userProfile);

        }

        // POST: UserProfile/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "userID,loginName,principalName,emailAddress,displayName")] UserProfile up)
        {
            if (ModelState.IsValid)
            {
                //db.Entry(userProfile).State = EntityState.Modified;
                UserProfile userProfile = db.UserProfiles.Find(up.userID);
                AllSorts.saveWordLists(Request.Form.GetValues("infaqkeywords"), Request.Form.GetValues("infaqexpertareas"), db, userProfile);
                AllSorts.saveWordLists(Request.Form.GetValues("inkbkeywords"), Request.Form.GetValues("inkbexpertareas"), db, userProfile, true);
                db.SaveChanges();
                //Session.Add("UserDisplayName", userProfile.displayName);//Update display name
                AllSorts.displayMessage = "Profile updated successfully!";

                return RedirectToAction("Index");
            } else
            {
                AllSorts.displayMessage = "0#General error updating User Profile.";
            }
            return View(up);
        }

        // GET: UserProfile/Create
        /***** Subscrpitons merged with profile (index)
        public ActionResult Subscriptions()
        {
            
            UserProfile userProfile = db.UserProfiles.Find(new Guid(AllSorts.getUserID()));

            if (Request.HttpMethod == "POST")
            {                

                db.Entry(userProfile).State = EntityState.Modified;                
                AllSorts.saveWordLists(Request.Form.GetValues("infaqkeywords"), Request.Form.GetValues("infaqexpertareas"), db, userProfile);
                AllSorts.saveWordLists(Request.Form.GetValues("inkbkeywords"), Request.Form.GetValues("inkbexpertareas"), db, userProfile, true);
                db.SaveChanges();

                AllSorts.displayMessage = "Subscriptions updated!";
            } 

            return View(userProfile);

        }              
        */

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
