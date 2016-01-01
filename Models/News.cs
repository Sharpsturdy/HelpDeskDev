using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Help_Desk_2.Models
{
    public class News
    {

        public int ID { get; set;  }

        [Required]
        [StringLength(150, ErrorMessage = "Your article title exceeds the maximum length allowed")]
        [Display(Name = "Article Title")]
        public string title { get; set; }

        [Display(Name ="Artilce Body")]
        [AllowHtml]
        [Required]
        public string body { get; set; }


        [Display(Name = "Sticky", Description = "Keeps this article at the top")]
        [DefaultValue(false)]
        public bool sticky { get; set; }

        [Display(Name = "Published")]
        [DefaultValue(false)]
        public bool published { get; set; } 

        [Display(Name = "Published on"), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        [DataType(DataType.DateTime)]
        public DateTime? publishedDate { get; set; }

        [ForeignKey("UserProfile")]
        public Guid originatorID { get; set; } //AD Username

        public virtual UserProfile UserProfile { get; set; }

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