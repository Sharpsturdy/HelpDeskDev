using System;
using Help_Desk_2.DataAccessLayer;
using Help_Desk_2.Models;
using Help_Desk_2.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Help_Desk_2.Controllers
{
    [CustomAuthorise(Roles =UserRoles.DomainAdminAndSuperUserRoles)]
    public class ReportsController : Controller
    {

        private HelpDeskContext db = new HelpDeskContext();


        // GET: Reports
        public ActionResult Index()
        {
            
            return View("Index",db.TicketKPIs);
        }


        public ActionResult OpenAssigned()
        {
            var ticketKPIs = db.TicketKPIs.Where(m => ((DateTime)m.dateCompleted == null && ((DateTime)m.lastAssigned != null)));
            
            if (ticketKPIs.Count() > 0)
            {
                ViewBag.OpenAssignedTotal = ticketKPIs.Count();
                List<kpidates> kpiDates = new List<kpidates>();

                foreach (TicketsKPI kpi in ticketKPIs)
                {
                    kpiDates.Add(new kpidates { _stoa = kpi.stoa, _stoc = kpi.TotalDaysToDate, _atoc = kpi.FromLastAssignedDays });
                }


                ViewBag.OpenAssignedAVGstoa = Math.Round(kpiDates.Average(s => s._stoa), 0);
                //List<int> Liststoa = ticketKPIs.s
                ViewBag.OpenAssignedAVGstoc = Math.Round(kpiDates.Average(s => s._stoc), 0);
                ViewBag.OpenAssignedAVGatoc = Math.Round(kpiDates.Average(s => s._atoc), 0);
            }

            return View("OpenAssigned", ticketKPIs);
        }


        public ActionResult YearToDateKPI()
        {

            
            var ticketKPIs = db.TicketKPIs.Where(m => ((DateTime)m.dateSubmitted).Year == DateTime.Now.Year);
            ViewBag.YTDTotal = ticketKPIs.Count();

            
            List<kpidates> kpiDates = new List<kpidates>();

            foreach (TicketsKPI kpi in ticketKPIs)
            {
                  kpiDates.Add(new kpidates { _stoa = kpi.stoa, _stoc = kpi.TotalDaysToDate, _atoc = kpi.FromLastAssignedDays });
            }
           
            ViewBag.YTDAVGstoa = Math.Round(kpiDates.Average(s => s._stoa),0);
            //List<int> Liststoa = ticketKPIs.s
            ViewBag.YTDAVGstoc = Math.Round(kpiDates.Average(s => s._stoc),0);
            ViewBag.YTDAVGatoc = Math.Round(kpiDates.Average(s => s._atoc),0);
            return View("YearToDateKPI", ticketKPIs);
        }

        public ActionResult MonthToDateKPI()
        {
            var ticketKPIs = db.TicketKPIs.Where(m => ((DateTime)m.dateSubmitted).Month == DateTime.Now.Month);
            ViewBag.MTDTotal = ticketKPIs.Count();
            List<kpidates> kpiDates = new List<kpidates>();

            foreach (TicketsKPI kpi in ticketKPIs)
            {
                kpiDates.Add(new kpidates { _stoa = kpi.stoa, _stoc = kpi.TotalDaysToDate, _atoc = kpi.FromLastAssignedDays });
            }


            ViewBag.MTDAVGstoa = Math.Round(kpiDates.Average(s => s._stoa), 0);
            //List<int> Liststoa = ticketKPIs.s
            ViewBag.MTDAVGstoc = Math.Round(kpiDates.Average(s => s._stoc), 0);
            ViewBag.MTDAVGatoc = Math.Round(kpiDates.Average(s => s._atoc), 0);
            return View("MonthToDateKPI", ticketKPIs);
        }


        public ActionResult UserMonthToDateKPI(string user, int? month)
        {

            List<UserProfile> list = db.UserProfiles.ToList();
            ViewBag.User = new SelectList(list, "principalName", "principalName");
           

            if (month == null || month == 0)
            { month = DateTime.Now.Month; }


            var ticketKPIs = db.TicketKPIs.Where(m => ((DateTime)m.dateSubmitted).Month == month);
            
            if (user != null && user != "")
            {
                //var userid = db.UserProfiles.Where(m => m.displayName == user).Select(x => x.userID);
                ticketKPIs = db.TicketKPIs.Where(m => ((DateTime)m.dateSubmitted).Month == month && m.responsible == user);
            }

            if (ticketKPIs.Count() > 0)
            {
                ViewBag.MTDTotal = ticketKPIs.Count();
                List<kpidates> kpiDates = new List<kpidates>();

                foreach (TicketsKPI kpi in ticketKPIs)
                {
                    kpiDates.Add(new kpidates { _stoa = kpi.stoa, _stoc = kpi.TotalDaysToDate, _atoc = kpi.FromLastAssignedDays });
                }


                ViewBag.MTDAVGstoa = Math.Round(kpiDates.Average(s => s._stoa), 0);
                //List<int> Liststoa = ticketKPIs.s
                ViewBag.MTDAVGstoc = Math.Round(kpiDates.Average(s => s._stoc), 0);
                ViewBag.MTDAVGatoc = Math.Round(kpiDates.Average(s => s._atoc), 0);
            }
            return View("UserMonthToDateKPI", ticketKPIs);
        }

        public ActionResult UserYearToDateKPI(string user, int? year)
        {

            List<UserProfile> list = db.UserProfiles.ToList();
            ViewBag.User = new SelectList(list, "principalName", "principalName");


            if (year == null || year == 0)
            { year = DateTime.Now.Year; }


            var ticketKPIs = db.TicketKPIs.Where(m => ((DateTime)m.dateSubmitted).Year == year);

            if (user != null && user != "")
            {
                //var userid = db.UserProfiles.Where(m => m.displayName == user).Select(x => x.userID);
                ticketKPIs = db.TicketKPIs.Where(m => ((DateTime)m.dateSubmitted).Year == year && m.responsible == user);
            }

            if (ticketKPIs.Count() > 0)
            {
                ViewBag.YTDTotal = ticketKPIs.Count();
                List<kpidates> kpiDates = new List<kpidates>();

                foreach (TicketsKPI kpi in ticketKPIs)
                {
                    kpiDates.Add(new kpidates { _stoa = kpi.stoa, _stoc = kpi.TotalDaysToDate, _atoc = kpi.FromLastAssignedDays });
                }


                ViewBag.YTDAVGstoa = Math.Round(kpiDates.Average(s => s._stoa), 0);
                //List<int> Liststoa = ticketKPIs.s
                ViewBag.YTDAVGstoc = Math.Round(kpiDates.Average(s => s._stoc), 0);
                ViewBag.YTDAVGatoc = Math.Round(kpiDates.Average(s => s._atoc), 0);
            }
            return View("UserYearToDateKPI", ticketKPIs);
        }
    }

    public class kpidates
    {
       public int _stoa { get; set; }
        public int _stoc { get; set; }
        public int _atoc { get; set; }
   
    }
}
