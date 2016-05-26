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

namespace Help_Desk_2.Controllers
{
    public class DummyWordListsController : Controller
    {
        private HelpDeskContext db = new HelpDeskContext();

        // GET: DummyWordLists
        public ActionResult Index()
        {
            return View(db.WordLists.ToList());
        }

        // GET: DummyWordLists/Details/5
        public ActionResult Details(int? id)
        {
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

        // GET: DummyWordLists/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DummyWordLists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,text,type,deleted")] WordList wordList)
        {
            if (ModelState.IsValid)
            {
                db.WordLists.Add(wordList);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(wordList);
        }

        // GET: DummyWordLists/Edit/5
        public ActionResult Edit(int? id)
        {
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

        // POST: DummyWordLists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,text,type,deleted")] WordList wordList)
        {
            if (ModelState.IsValid)
            {
                db.Entry(wordList).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(wordList);
        }

        // GET: DummyWordLists/Delete/5
        public ActionResult Delete(int? id)
        {
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

        // POST: DummyWordLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            WordList wordList = db.WordLists.Find(id);
            db.WordLists.Remove(wordList);
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
}
