using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Help_Desk_2.DataAccessLayer;
using Help_Desk_2.Models;
using Help_Desk_2.Utilities;

namespace Help_Desk_2.Controllers
{
    [CustomAuthorise(Roles = "Administrators")]
    public class GlobalSettingsController : Controller
    {
        private HelpDeskContext db = new HelpDeskContext();

        // GET: GlobalSettings, get first record
        public ActionResult Index()
        {
            GlobalSettings gs = db.GlobalSettingss.FirstOrDefault<GlobalSettings>();
           
            //if (gs == null) { }
            ViewBag.faqapprovers = new MultiSelectList(AllSorts.AllUsers, "userID", "displayName", AllSorts.AllUsers.Where(x => x.isFaqApprover).Select(u=>u.userID));
            ViewBag.kbapprovers = new MultiSelectList(AllSorts.AllUsers, "userID", "displayName", AllSorts.AllUsers.Where(x => x.isKbApprover).Select(u => u.userID));
            ViewBag.adminemails = new MultiSelectList(AllSorts.AllUsers, "userID", "displayName", AllSorts.AllUsers.Where(x => x.isResponsible).Select(u => u.userID));

            return View(gs);
        }

        // POST: GlobalSettings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "ID,TicketSeeder,TicketHeader,TicketExpiryDays,KBFAQsExpiryDays")] GlobalSettings globalSettings)
        {
            
            if (ModelState.IsValid)
            {

                if (globalSettings.ID == Guid.Empty || globalSettings.ID == null)
                {
                    globalSettings.ID = Guid.NewGuid();
                    db.GlobalSettingss.Add(globalSettings);
                }
                else {
                    db.Entry(globalSettings).State = EntityState.Modified;
                }

                AllSorts.saveGSLists(Request.Form.GetValues("faqapprovers"), db, 1);
                AllSorts.saveGSLists(Request.Form.GetValues("kbapprovers"), db, 2);
                AllSorts.saveGSLists(Request.Form.GetValues("adminemails"), db, 3);

                db.SaveChanges();

                ViewBag.Msg = "Changes saved";
                return RedirectToAction("Index");
            } else
            {
                ViewBag.Msg = "Model not valid";
            }

            ViewBag.faqapprovers = new MultiSelectList(AllSorts.AllUsers, "userID", "displayName", AllSorts.AllUsers.Where(x => x.isFaqApprover).Select(u => u.userID));
            ViewBag.kbapprovers = new MultiSelectList(AllSorts.AllUsers, "userID", "displayName", AllSorts.AllUsers.Where(x => x.isKbApprover).Select(u => u.userID));
            ViewBag.adminemails = new MultiSelectList(AllSorts.AllUsers, "userID", "displayName", AllSorts.AllUsers.Where(x => x.isResponsible).Select(u => u.userID));

            return View(globalSettings);
        }

        /********* Keywords Section ****************/
        public ActionResult Keywords()
        {
            
            return View(db.WordLists.Where(k => k.type == 1).ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Keywords([Bind(Include = "type,text")] WordList wordList)
        {
            if (ModelState.IsValid)
            {
                db.WordLists.Add(wordList);
                db.SaveChanges();
            }
            return RedirectToAction("Keywords");
        }

        /********* Expert Area Section ****************/
        public ActionResult ExpertAreas()
        {
            
            return View(db.WordLists.Where(k => k.type == 2).ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExpertAreas([Bind(Include = "type,text")] WordList wordList)
        {
            //wordList.type = 1; //Since it is a keyword
            if (ModelState.IsValid)
            {
                db.WordLists.Add(wordList);
                db.SaveChanges();
            }
            return RedirectToAction("ExpertAreas");
        }
        /*
        // GET: GlobalSettings/Create
        public ActionResult Create()
        {
            return View();
        }
        
        // POST: GlobalSettings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,AdminEmail,TicketSeeder,FAQApprover,KBApprover,TicketHeader,Keyowrds,ExpertArea,TicketExpiry")] GlobalSettings globalSettings)
        {
            if (ModelState.IsValid)
            {
                globalSettings.ID = Guid.NewGuid();
                db.GlobalSettingss.Add(globalSettings);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(globalSettings);
        }
        
        // GET: GlobalSettings/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GlobalSettings globalSettings = db.GlobalSettingss.Find(id);
            if (globalSettings == null)
            {
                return HttpNotFound();
            }
            return View(globalSettings);
        }

        // POST: GlobalSettings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,AdminEmail,TicketSeeder,FAQApprover,KBApprover,TicketHeader,Keyowrds,ExpertArea,TicketExpiry")] GlobalSettings globalSettings)
        {
            if (ModelState.IsValid)
            {
                db.Entry(globalSettings).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(globalSettings);
        }

        // GET: GlobalSettings/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GlobalSettings globalSettings = db.GlobalSettingss.Find(id);
            if (globalSettings == null)
            {
                return HttpNotFound();
            }
            return View(globalSettings);
        }

        // POST: GlobalSettings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            GlobalSettings globalSettings = db.GlobalSettingss.Find(id);
            db.GlobalSettingss.Remove(globalSettings);
            db.SaveChanges();
            return RedirectToAction("Index");
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
