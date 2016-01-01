using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Help_Desk_2.Models
{
    public class News
    {

        public int ID { get; set;  }

        [MaxLength(50)]
        [Display(Name = "Title")]
        public string title { get; set; }

        [Display(Name ="Body")]
        [AllowHtml]
        public string body { get; set; }


        [Display(Name = "Sticky")]
        [DefaultValue(false)]
        public bool sticky { get; set; }

        [Display(Name = "Published")]
        [DefaultValue(false)]
        public bool published { get; set; } 

        [Display(Name = "Published on")]
        public DateTime publishedDate { get; set; }

        [Display(Name = "Created on")]
        public DateTime creationDate {
            get { return creationDate; }
            set {
                if (creationDate == null)
                {
                    creationDate = DateTime.Now;
                }
            }
        }


    }
}