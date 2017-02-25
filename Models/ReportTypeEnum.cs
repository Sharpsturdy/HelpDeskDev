using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Help_Desk_2.Models
{
    public enum ReportTypeEnum
    {
        Summary     = 1,
        MonthToDate = 2,
        YearToDate  = 3,
        MonthToDateByUser = 4,
        YearToDateByUser = 5,
        OpenAssigned = 6
    }
}