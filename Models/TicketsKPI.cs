﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Help_Desk_2.Models
{
    public class TicketsKPI
    {
        [Key]
        public int ID { get; set; }
        public int ticketID { get; set; } 
        public string responsible { get; set; }
        [Display(Name = "dateSubmitted"), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]

        public DateTime? dateSubmitted { get; set; }
        [Display(Name = "dateCompleted"), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? dateCompleted { get; set; }
        [Display(Name = "lastAssigned"), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]

        public DateTime? lastAssigned { get; set; }

        [NotMapped]
        [Display(Name = "SubmittedToAssigned")]
        public int stoa
        {
            get
            {
                if (dateSubmitted != null  && lastAssigned != null)
                {
                    //DateTime la = (DateTime)lastAssigned;
                    TimeSpan difference = (DateTime)lastAssigned - (DateTime)dateSubmitted;
                    var days = difference.Days;
                    return days;
                }
                else
                {
                    return 0;

                }

            }
          

        }

        [NotMapped]
        [Display(Name = "AssignedToCompleted")]
        public int FromLastAssignedDays
        {
            get
            {
                DateTime reportedCompletedDate = dateCompleted ?? DateTime.Now;
                if (lastAssigned.HasValue)
                {
                    TimeSpan difference = reportedCompletedDate - lastAssigned.Value;
                    int days = difference.Days;
                    return days;
                }
                else
                {
                    return 0;

                }

            }


        }

        [NotMapped]
        [Display(Name = "SubmittedToCompleted")]
        public int TotalDaysToDate
        {
            get
            {
                DateTime reportedCompletedDate = dateCompleted ?? DateTime.Now;
                if (dateSubmitted.HasValue)
                {
                    TimeSpan difference = reportedCompletedDate - dateSubmitted.Value;
                    int days = difference.Days;
                    return days;
                }
                else
                {
                    return 0;

                }

            }


        }

        [NotMapped]
        [Display(Name = "DateCompleted"), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime DateCompletedForTable
        {
            get
            {
                if (dateCompleted.HasValue)
                {
                    return dateCompleted.Value;
                }
                else
                {
                    return DateTime.Now;

                }
            }
        }
    }
}