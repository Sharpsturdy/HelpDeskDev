using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Help_Desk_2.DataAccessLayer;
using Help_Desk_2.ViewModels;
using Help_Desk_2.Models;

namespace Help_Desk_2.Controllers
{
    public class WordListController : Controller
    {
        private HelpDeskContext db = new HelpDeskContext();

        // GET: WordList
        public ActionResult Index()
        {
            return View(db.WordLists.ToList());
        }

        // GET: WordList CreateKeyword
        public ActionResult CreateKeyword()
        {
            return View();
        }

        // GET: WordList/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WordList/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,text,type")] WordListViewModel wlm)
        {
            WordList wordList = new WordList();
            wordList.text = wlm.text;
            wordList.type = 1;

            if (ModelState.IsValid)
            {
                db.WordLists.Add(wordList);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(wlm);
        }

        // GET: WordList/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WordListViewModel wordListViewModel = db.WordListViewModels.Find(id);
            if (wordListViewModel == null)
            {
                return HttpNotFound();
            }
            return View(wordListViewModel);
        }

        // POST: WordList/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,text,type")] WordListViewModel wordListViewModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(wordListViewModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(wordListViewModel);
        }

        // GET: WordList/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WordListViewModel wordListViewModel = db.WordListViewModels.Find(id);
            if (wordListViewModel == null)
            {
                return HttpNotFound();
            }
            return View(wordListViewModel);
        }

        // POST: WordList/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            WordListViewModel wordListViewModel = db.WordListViewModels.Find(id);
            db.WordListViewModels.Remove(wordListViewModel);
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
