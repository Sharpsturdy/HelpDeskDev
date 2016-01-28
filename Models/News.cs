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

        [Display(Name = "Published on"), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        public DateTime? publishedDate { get; set; }

        [ForeignKey("Originator")]
        public Guid originatorID { get; set; } //AD Username

        public virtual UserProfile Originator { get; set; }

        [Display(Name = "Created on"), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime creationDate {  get;  set; }

        [NotMapped]
        public string status
        {
            get
            {
                if (published)
                    return "Published";
                else
                    return "Unpublished";
            }
        }

        [NotMapped]
        public string dspSticky
        {
            get
            {
                 return sticky ? "Yes": "No";
            }
        }

        [NotMapped]
        public string dspPublished
        {
            get
            {
                return published ? "Yes" : "No";
            }
        }

    }
}