using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Help_Desk_2.Controllers
{
    public class FAQsController : Controller
    {
        // GET: FAQs
        public ActionResult Index()
        {
            return View();
        }

        // GET: FAQs/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: FAQs/Create
        public ActionResult Create()
        {
            return View();
        }


        // POST: FAQs/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: FAQs/Suggest
        public ActionResult Suggest()
        {
            return View();
        }

        // GET: FAQs/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: FAQs/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: FAQs/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: FAQs/Delete/5
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
