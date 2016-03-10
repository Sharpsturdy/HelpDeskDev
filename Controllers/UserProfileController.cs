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

namespace Help_Desk_2.Controllers
{
    public class UserProfileController : Controller
    {
        private HelpDeskContext db = new HelpDeskContext();

        // GET: UserProfile
        public ActionResult List()
        {
            return View(db.UserProfiles.ToList());
        }

        // GET: UserProfile/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserProfile userProfile = db.UserProfiles.Find(id);
            if (userProfile == null)
            {
                return HttpNotFound();
            }
            return View(userProfile);
        }

        // GET: UserProfile/Create
        public ActionResult Index()
        {
            //UserData ud = new UserData();
            //UserProfile userProfile = ud.getUserProfile();

            UserProfile userProfile = db.UserProfiles.Find(new Guid(AllSorts.getUserID()));

            return View(userProfile);

        }

        // GET: UserProfile/Create
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

        

        // POST: UserProfile/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Indexn([Bind(Include = "userID,loginName,principalName,firstName,surName,emailAddress,contactNumber")] UserProfile userProfile)
        {
            if (ModelState.IsValid)
            {
                userProfile.userID = Guid.NewGuid();
                db.UserProfiles.Add(userProfile);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(userProfile);
        }

        // GET: UserProfile/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserProfile userProfile = db.UserProfiles.Find(id);
            
            if (userProfile == null)
            {
                return HttpNotFound();
            }
            return View(userProfile);
        }

        // POST: UserProfile/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "userID,loginName,principalName,firstName,surName,emailAddress,contactNumber")] UserProfile userProfile)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userProfile).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(userProfile);
        }

        
        // POST: UserProfile/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            UserProfile userProfile = db.UserProfiles.Find(id);
            db.UserProfiles.Remove(userProfile);
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


    }

    public class OLD_UserData {

        public string loginName { get; set; }

        public string displayName { get; set;  }

        public string userPN { get; set; }

        public Guid guid { get; set;  }

        public string firstName { get; set;  }

        public string lastName { get; set; }

        private HelpDeskContext db = new HelpDeskContext();

        public OLD_UserData()
        {
            var user = HttpContext.Current.User;
            loginName = user.Identity.Name;

            PrincipalContext ctx;

            if (loginName.IndexOf(@"\") > 0)
            {
                ctx = new PrincipalContext(ContextType.Domain, loginName.Substring(0, loginName.IndexOf(@"\"))); //"vmwin2008svr1"); //
            }
            else
            {
                ctx = new PrincipalContext(ContextType.Domain);
            }

            var userPrincipal = UserPrincipal.FindByIdentity(ctx,loginName);

            try
            {
                if (user != null)
                {
                    displayName = userPrincipal.DisplayName;
                    userPN = userPrincipal.UserPrincipalName;
                    guid = (Guid) userPrincipal.Guid;
                    firstName = userPrincipal.GivenName;
                    lastName = userPrincipal.Surname;
                }

                //var WinUser = new WindowsIdentity(userPN);
                
            }
            catch (Exception ex)
            {
                //if (Globals.debug) Globals.form1Err += "Part2 Error: " + ex.Message;
            }

            

        }
    }
}
