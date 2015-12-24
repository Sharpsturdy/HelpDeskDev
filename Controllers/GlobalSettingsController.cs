using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Help_Desk_2.DataAccessLayer;
using Help_Desk_2.Models;
using Help_Desk_2.ViewModels;

namespace Help_Desk_2.Controllers
{
    public class GlobalSettingsController : Controller
    {
        private HelpDeskContext db = new HelpDeskContext();

        // GET: GlobalSettings, get first record
        public ActionResult Index()
        {
            GlobalSettingsEditModel gsm = null;
            GlobalSettings globalSettings = db.GlobalSettingss.FirstOrDefault<GlobalSettings>();
            
            if (globalSettings == null || globalSettings.ID == null)
            {
                ViewBag.Msg = "ID is blank. This must be a new record!";
                gsm = new GlobalSettingsEditModel();
            } else
            {
                gsm = new GlobalSettingsEditModel(globalSettings);
            }

            var kwds = from kwd in db.WordLists
                               where kwd.type == 1
                               select kwd.text;

            ViewBag.keywords = kwds; // String.Join(", ", kwds);

            var eAreas = from eArea in db.WordLists
                         where eArea.type == 2
                         select eArea.text;

            ViewBag.expertareas = eAreas; // String.Join(", ", eAreas);
            return View(gsm);


        }

        // POST: GlobalSettings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "ID,AdminEmail,TicketSeeder,FAQApprover,KBApprover,TicketHeader,Keyowrds,ExpertArea,TicketExpiry")] GlobalSettings globalSettings)
        {
            
            if (ModelState.IsValid)
            {
                //GlobalSettings globalSettings = gsm.getSettings();
               

                if (globalSettings.ID == Guid.Empty || globalSettings.ID == null)
                {
                    globalSettings.ID = Guid.NewGuid();
                    db.GlobalSettingss.Add(globalSettings);
                }
                else {
                    db.Entry(globalSettings).State = EntityState.Modified;
                }
                db.SaveChanges();

                ViewBag.Msg = "Changes saved";
                return RedirectToAction("Index");
            } else
            {
                ViewBag.Msg = "Model not valid";
            }

            GlobalSettingsEditModel gsm = new GlobalSettingsEditModel(globalSettings);
            return View(gsm);
        }

        /********* Keywords Section ****************/
        public ActionResult Keywords()
        {
            var kwds = from kwd in db.WordLists
                                  where kwd.type == 1
                                  select kwd;

            return View(kwds.ToList<WordList>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Keywords([Bind(Include = "text")] WordList wordList)
        {
            if (ModelState.IsValid)
            {
                db.WordLists.Add(wordList);
                db.SaveChanges();
                return RedirectToAction("Keywords");
            }
            return RedirectToAction("Keywords");
        }

        /********* Expert Area Section ****************/
        public ActionResult ExpertAreas()
        {
            var eAreas = from eArea in db.WordLists
                         where eArea.type == 2
                         select eArea;
            return View(eAreas.ToList());
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
                return RedirectToAction("ExpertAreas");
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
