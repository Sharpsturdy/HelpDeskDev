using System;
using Help_Desk_2.DataAccessLayer;
using Help_Desk_2.Models;
using Help_Desk_2.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Expressions;

namespace Help_Desk_2.Controllers
{
    [CustomAuthorise(Roles =UserRoles.DomainAdminAndSuperUserRoles)]
    public class ReportsController : Controller
    {

        private HelpDeskContext db = new HelpDeskContext();


        //// GET: Reports
        //public ActionResult Index()
        //{
            
        //    return View("Index",db.TicketKPIs);
        //}


        //public ActionResult OpenAssigned()
        //{
        //    var ticketKPIs = db.TicketKPIs.Where(m => (!m.dateCompleted.HasValue && m.lastAssigned.HasValue));

        //    if (ticketKPIs.Count() > 0)
        //    {
        //        ViewBag.OpenAssignedTotal = ticketKPIs.Count();
        //        List<kpidates> kpiDates = new List<kpidates>();

        //        foreach (TicketsKPI kpi in ticketKPIs)
        //        {
        //            kpiDates.Add(new kpidates { _stoa = kpi.stoa, _stoc = kpi.TotalDaysToDate, _atoc = kpi.FromLastAssignedDays });
        //        }


        //        ViewBag.OpenAssignedAVGstoa = Math.Round(kpiDates.Average(s => s._stoa), 0);
        //        //List<int> Liststoa = ticketKPIs.s
        //        ViewBag.OpenAssignedAVGstoc = Math.Round(kpiDates.Average(s => s._stoc), 0);
        //        ViewBag.OpenAssignedAVGatoc = Math.Round(kpiDates.Average(s => s._atoc), 0);
        //    }

        //    return View("OpenAssigned", ticketKPIs);
        //}


        //public ActionResult YearToDateKPI()
        //{
        //    Func<TicketsKPI, bool> selectFunc = (t) => (t.dateSubmitted?.Year == DateTime.Now.Year)
        //                                        || (t.dateCompleted?.Year == DateTime.Now.Year)
        //                                        || !t.dateCompleted.HasValue;

        //    var ticketKPIs = db.TicketKPIs.Where(selectFunc);
        //    ViewBag.YTDTotal = ticketKPIs.Count();

            
        //    List<kpidates> kpiDates = new List<kpidates>();

        //    foreach (TicketsKPI kpi in ticketKPIs)
        //    {
        //          kpiDates.Add(new kpidates { _stoa = kpi.stoa, _stoc = kpi.TotalDaysToDate, _atoc = kpi.FromLastAssignedDays });
        //    }
           
        //    ViewBag.YTDAVGstoa = Math.Round(kpiDates.Average(s => s._stoa),0);
        //    //List<int> Liststoa = ticketKPIs.s
        //    ViewBag.YTDAVGstoc = Math.Round(kpiDates.Average(s => s._stoc),0);
        //    ViewBag.YTDAVGatoc = Math.Round(kpiDates.Average(s => s._atoc),0);
        //    return View("YearToDateKPI", ticketKPIs);
        //}

        //public ActionResult MonthToDateKPI()
        //{
        //    Func<TicketsKPI, bool> selectFunc = (t) => (t.dateSubmitted?.Month == DateTime.Now.Month)
        //                                        || (t.dateCompleted?.Month == DateTime.Now.Month)
        //                                        || !t.dateCompleted.HasValue;
        //    var ticketKPIs = db.TicketKPIs.Where(selectFunc);
        //    ViewBag.MTDTotal = ticketKPIs.Count();
        //    List<kpidates> kpiDates = new List<kpidates>();
        //    foreach (TicketsKPI kpi in ticketKPIs)
        //    {
        //        kpiDates.Add(new kpidates { _stoa = kpi.stoa, _stoc = kpi.TotalDaysToDate, _atoc = kpi.FromLastAssignedDays });
        //    }
        //    ViewBag.MTDAVGstoa = Math.Round(kpiDates.Average(s => s._stoa), 0);
        //    //List<int> Liststoa = ticketKPIs.s
        //    ViewBag.MTDAVGstoc = Math.Round(kpiDates.Average(s => s._stoc), 0);
        //    ViewBag.MTDAVGatoc = Math.Round(kpiDates.Average(s => s._atoc), 0);
        //    return View("MonthToDateKPI", ticketKPIs);
        //}


        public ActionResult Index(string user, int? month, int? year, TicketsKpiStatus? status) //Previus acion method called UserMonthToDateKPI
        {

            List<UserProfile> list = db.UserProfiles.ToList();
            ViewBag.User = new SelectList(list, "principalName", "principalName");
            ViewBag.Years = GetYearsForDropdowns();
            
           

            if (month == null || month == 0) { month = DateTime.Now.Month; }
            if (year ==  null || year  == 0) { year  = DateTime.Now.Year; }
            if (status == null ) { status = TicketsKpiStatus.All; }

            DateTime reportDate = GetReportDate(year.Value, month.Value);
            ViewBag.ReportDate = reportDate;

            if (IsCorrectPeriodRequested(year.Value, month.Value))
            {
                IQueryable<TicketsKPI> selectedByResponsible = string.IsNullOrEmpty(user) ? db.TicketKPIs : db.TicketKPIs.Where(m => m.responsible == user);
                IEnumerable<TicketsKPI> ticketKPIs = SelectByStatus(selectedByResponsible, status.Value, year.Value, month.Value);
                               
                if (ticketKPIs.Count() > 0)
                {
                    ViewBag.MTDTotal = ticketKPIs.Count();
                    List<kpidates> kpiDates = new List<kpidates>();

                    foreach (TicketsKPI kpi in ticketKPIs)
                    {
                        kpiDates.Add(new kpidates { _stoa = kpi.stoa, _stoc = kpi.TotalDaysToDate(reportDate), _atoc = kpi.FromLastAssignedDays(reportDate) });
                    }


                    ViewBag.MTDAVGstoa = Math.Round(kpiDates.Average(s => s._stoa), 0);
                    //List<int> Liststoa = ticketKPIs.s
                    ViewBag.MTDAVGstoc = Math.Round(kpiDates.Average(s => s._stoc), 0);
                    ViewBag.MTDAVGatoc = Math.Round(kpiDates.Average(s => s._atoc), 0);
                }
                return View("UserMonthToDateKPI", ticketKPIs);
            }
            return View("UserMonthToDateKPI", new List<TicketsKPI>());
        }

        private bool IsCorrectPeriodRequested(int year, int month)
        {
            DateTime currentMonthStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime reportPeriodStart = new DateTime(year, month, 1);
            return reportPeriodStart < currentMonthStart.AddMonths(1);            
        }

        private DateTime GetReportDate(int year, int month)
        {
            var currentDate = DateTime.Now;
            if(year==currentDate.Year && month == currentDate.Month)
            {
                return DateTime.Now;
            }
            return new DateTime(year, month, 1).AddMonths(1).AddDays(-1);
        }

        private IEnumerable<TicketsKPI> SelectByStatus(IEnumerable<TicketsKPI> ticketKPIs, TicketsKpiStatus status, int year, int month)
        {
            DateTime monthStart = new DateTime(year, month, 1);
            Func<TicketsKPI, bool> selectByStatusFunc;
            switch (status)
            {
                case TicketsKpiStatus.All:
                    if (year == DateTime.Now.Year && month == DateTime.Now.Month)
                    {
                        selectByStatusFunc = (t) => ((t.dateSubmitted?.Month == month) && (t.dateSubmitted?.Year == year))
                                                 || ((t.dateCompleted?.Month == month) && (t.dateCompleted?.Year == year))
                                                 || (!t.dateCompleted.HasValue && t.dateSubmitted < monthStart);
                    }
                    else
                    {
                        selectByStatusFunc = (t) => ((t.dateSubmitted?.Month == month) && (t.dateSubmitted?.Year == year))
                                                 || ((t.dateCompleted?.Month == month) && (t.dateCompleted?.Year == year));
                    }
                    break;
                case TicketsKpiStatus.Open:
                    if (year == DateTime.Now.Year && month == DateTime.Now.Month)
                    {
                        selectByStatusFunc = (t) => (((t.dateSubmitted?.Month == month) && (t.dateSubmitted?.Year == year) && (t.dateCompleted?.Month != month && t.dateCompleted?.Year != year))
                                                  || (!t.dateCompleted.HasValue && t.dateSubmitted < monthStart));
                    }
                    else
                    {
                        selectByStatusFunc = (t) => ((t.dateSubmitted?.Month == month) && (t.dateSubmitted?.Year == year) && (t.dateCompleted?.Month != month && t.dateCompleted?.Year != year));
                                                  
                    }
                    break;
                case TicketsKpiStatus.Closed:
                    selectByStatusFunc = (t) => (t.dateCompleted?.Month == month) && (t.dateCompleted?.Year == year);
                    break;
                default:
                    throw new ArgumentException("Unexpected ticket KPI status");
            }
            return ticketKPIs.Where(selectByStatusFunc);
        }

        private IEnumerable<SelectListItem> GetYearsForDropdowns()
        {
           List<SelectListItem> years =  db.TicketKPIs.Where(t => t.dateSubmitted.HasValue)
                                                      .Select(t => t.dateSubmitted.Value.Year)
                                                      .Distinct()
                                                      .Select(y => new SelectListItem() { Value = y.ToString(), Text = y.ToString() })
                                                      .ToList();
            years.Insert(0, new SelectListItem() { Value = DateTime.Now.Year.ToString(), Text = "Current year" });
            return years;
        }

        //public ActionResult UserYearToDateKPI(string user, int? year)
        //{

        //    List<UserProfile> list = db.UserProfiles.ToList();
        //    ViewBag.User = new SelectList(list, "principalName", "principalName");


        //    if (year == null || year == 0)
        //    { year = DateTime.Now.Year; }

        //    Func<TicketsKPI, bool> selectFunc = (t) => (t.dateSubmitted?.Year == year)
        //                                      || (t.dateCompleted?.Year == year)
        //                                      || !t.dateCompleted.HasValue;

        //    var ticketKPIs = db.TicketKPIs.Where(selectFunc);

        //    if (user != null && user != "")
        //    {
        //        //var userid = db.UserProfiles.Where(m => m.displayName == user).Select(x => x.userID);
        //        ticketKPIs = db.TicketKPIs.Where(m => ((DateTime)m.dateSubmitted).Year == year && m.responsible == user);
        //    }

        //    if (ticketKPIs.Count() > 0)
        //    {
        //        ViewBag.YTDTotal = ticketKPIs.Count();
        //        List<kpidates> kpiDates = new List<kpidates>();

        //        foreach (TicketsKPI kpi in ticketKPIs)
        //        {
        //            kpiDates.Add(new kpidates { _stoa = kpi.stoa, _stoc = kpi.TotalDaysToDate, _atoc = kpi.FromLastAssignedDays });
        //        }

        //        ViewBag.YTDAVGstoa = Math.Round(kpiDates.Average(s => s._stoa), 0);
        //        //List<int> Liststoa = ticketKPIs.s
        //        ViewBag.YTDAVGstoc = Math.Round(kpiDates.Average(s => s._stoc), 0);
        //        ViewBag.YTDAVGatoc = Math.Round(kpiDates.Average(s => s._atoc), 0);
        //    }
        //    return View("UserYearToDateKPI", ticketKPIs);
        //}
    }

    public class kpidates
    {
       public int _stoa { get; set; }
        public int _stoc { get; set; }
        public int _atoc { get; set; }
   
    }
}
