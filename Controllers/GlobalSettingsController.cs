using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Help_Desk_2.DataAccessLayer;
using Help_Desk_2.Models;
using Help_Desk_2.Utilities;
using MvcPaging;
using System.Net;

namespace Help_Desk_2.Controllers
{
    //[CustomAuthorise(Roles = "AdminUsers")]
    public class GlobalSettingsController : Controller
    {
        private HelpDeskContext db = new HelpDeskContext();

        // GET: GlobalSettings, get first record
        public ActionResult Index()
        {
            if (!AllSorts.userHasRole("AdminUsers"))
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

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
        public ActionResult Index([Bind(Include = "ID,TicketSeeder,TicketHeader,TicketHeaderEnabled,TicketExpiryDays,FAQsExpiryDays,KBExpiryDays")] GlobalSettings globalSettings)
        {
            if (!AllSorts.userHasRole("AdminUsers"))
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);


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

                AllSorts.saveGSLists(db,Request.Form.GetValues("faqapprovers"), 1);
                AllSorts.saveGSLists(db,Request.Form.GetValues("kbapprovers"), 2);
                AllSorts.saveGSLists(db,Request.Form.GetValues("adminemails"), 3);

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.faqapprovers = new MultiSelectList(AllSorts.AllUsers, "userID", "displayName", AllSorts.AllUsers.Where(x => x.isFaqApprover).Select(u => u.userID));
            ViewBag.kbapprovers = new MultiSelectList(AllSorts.AllUsers, "userID", "displayName", AllSorts.AllUsers.Where(x => x.isKbApprover).Select(u => u.userID));
            ViewBag.adminemails = new MultiSelectList(AllSorts.AllUsers, "userID", "displayName", AllSorts.AllUsers.Where(x => x.isResponsible).Select(u => u.userID));

            return View(globalSettings);
        }

        /********* Keywords Section ****************/
        public ActionResult Keywords(int? page)
        {
            if (!AllSorts.userHasRole("AdminUsers"))
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;

            return View("Keywords",db.WordLists.Where(k => k.type == 1 && !k.deleted)
                    .OrderBy(k => k.text)
                    .ToPagedList(currentPageIndex, AllSorts.pageSize));
           // return View(db.WordLists.Where(k => k.type == 1).ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Keywords([Bind(Include = "type,text")] WordList wordList)
        {
            if (!AllSorts.userHasRole("AdminUsers"))
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

            if (ModelState.IsValid)
            {
                db.WordLists.Add(wordList);
                db.SaveChanges();
            }
            return RedirectToAction("Keywords");
        }

        public ActionResult KeywordsDeleted(int? page)
        {
            if (!AllSorts.userHasRole("AdminUsers"))
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;

            return View("Keywords", db.WordLists.Where(k => k.type == 1 && k.deleted)
                    .OrderBy(k => k.text)
                    .ToPagedList(currentPageIndex, AllSorts.pageSize));
            // return View(db.WordLists.Where(k => k.type == 1).ToList());
        }

        /********* Expert Area Section ****************/
        public ActionResult ExpertAreas(int? page)
        {
            if (!AllSorts.userHasRole("AdminUsers"))
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;

            return View("ExpertAreas",db.WordLists.Where(k => k.type == 2 && !k.deleted)
                    .OrderBy(k => k.text)
                    .ToPagedList(currentPageIndex, AllSorts.pageSize));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExpertAreas([Bind(Include = "type,text")] WordList wordList)
        {
            if (!AllSorts.userHasRole("AdminUsers"))
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

            //wordList.type = 1; //Since it is a keyword
            if (ModelState.IsValid)
            {
                db.WordLists.Add(wordList);
                db.SaveChanges();
            }
            return RedirectToAction("ExpertAreas");
        }

        /********* Expert Area Section ****************/
        public ActionResult ExpertAreasDeleted(int? page)
        {
            if (!AllSorts.userHasRole("AdminUsers"))
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;

            return View("ExpertAreas", db.WordLists.Where(k => k.type == 2 && k.deleted)
                    .OrderBy(k => k.text)
                    .ToPagedList(currentPageIndex, AllSorts.pageSize));
        }

        // GET: DummyWordLists/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!AllSorts.userHasRole("AdminUsers"))
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WordList wordList = db.WordLists.Find(id);
            if (wordList == null)
            {
                return HttpNotFound();
            }
            return View(wordList);
        }

        // POST: WordLists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,text,type,deleted")] WordList wordList)
        {
            if (!AllSorts.userHasRole("AdminUsers"))
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

            if (ModelState.IsValid)
            {
                db.Entry(wordList).State = EntityState.Modified;
                db.SaveChanges();
                AllSorts.displayMessage = (wordList.type == 1 ? "Keyword" : "Expert Area") + " updated successfully!";
                return RedirectToAction(wordList.type ==1 ? "Keywords":"ExpertAreas");
            }
            return View(wordList);
        }

        // GET: WordLists/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!AllSorts.userHasRole("AdminUsers"))
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WordList wordList = db.WordLists.Find(id);
            string type = wordList.type ==1 ? "Keyword":"Expert Area";
            
            db.Entry(wordList).State = EntityState.Modified;
            wordList.deleted = true;

            db.SaveChanges();
            AllSorts.displayMessage = type + " '" + wordList.text + "' has been deleted successfully!";
            return RedirectToAction(type.Replace(" ","") + "s");
        }


        // GET: WordLists/UnDelete/5
        public ActionResult UnDelete(int? id)
        {
            if (!AllSorts.userHasRole("AdminUsers"))
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WordList wordList = db.WordLists.Find(id);
            string type = wordList.type == 1 ? "Keyword" : "Expert Area";

            db.Entry(wordList).State = EntityState.Modified;
            wordList.deleted = false;

            db.SaveChanges();
            AllSorts.displayMessage = type + " '" + wordList.text + "' has been restored successfully!";
            return RedirectToAction(type.Replace(" ", "") + "sDeleted");
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
