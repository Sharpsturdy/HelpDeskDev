using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Help_Desk_2.Models;
using Help_Desk_2.DataAccessLayer;

namespace Help_Desk_2.Models
{
    public class KnowledgeFAQ
    {
        public int ID { get; set;  }

        [Display(Name = "Created on:"), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        [DataType(DataType.DateTime)]
        public DateTime dateComposed { get; set; }

        [Display(Name = "Expiries on:"), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        [DataType(DataType.DateTime)]
        public DateTime? exiryDate { get; set;  }

        [Required]
        [Display(Name = "Brief description")]
        [StringLength(150, ErrorMessage = "Your brief description exceeds the maximum characters allowed")]
        public string headerText { get; set; }

        [Required]
        [AllowHtml]
        [Display(Name = "Detailed description")]
        [DataType(DataType.MultilineText)]       
        public string description { get; set; }

        [ForeignKey("UserProfile")]
        public Guid originatorID { get; set; } //AD Username

        [Display(Name = "Links")]
        public string links { get; set; }

        [NotMapped]
        public string linkText { get; set; }

        [NotMapped]
        public string linkURL { get; set; }

        public virtual UserProfile UserProfile { get; set; }

        public virtual ICollection<Attachment> Files { get; set; }

        [NotMapped]     //List of files to delete
        public string deleteField { get; set; }

        public byte type { get; set; } //type => 1=FAQs, 2=Knowledge Base

        [NotMapped]
        [Display(Name = "Status")]
        public string status
        {
            get
            {
                return "Draft";
            }
        }

        public virtual ICollection<WordList> wordList { get; set; }

        [NotMapped]
        [Display(Name = "Keywords", Prompt = "Select keywords from list")]
        public IEnumerable<WordList> keywords {
            get {
                return wordList.Where(x => x.type == 1);
            }
            set { keywords = value; }
        }

        [NotMapped]
        [Display(Name = "Expert Areas", Prompt = "Select expert areas from list")]
        public IEnumerable<WordList> expertAreas
        {
            get
            {
                return wordList.Where(x => x.type == 2);
            }
            set { expertAreas = value; }
        }
        
              
        /*[NotMapped]
        [Display(Name = "Keywords", Prompt = "Select keywords from list")]
        public List<int> keywordIDs {
            get {
                return wordList.Where(x => x.type==1).Select(x=>x.ID).ToList();
            }
            set { keywordIDs = value; }
        }

        [NotMapped]
        [Display(Name = "Expert Areas", Prompt = "Select expert areas from list")]
        public List<int> expertAreaIDs
        {
            get
            {
                return wordList.Where(x => x.type == 2).Select(x => x.ID).ToList();
            }
            set { expertAreaIDs = value; }
        }
        */

        //read-only faq approverID

    }
}