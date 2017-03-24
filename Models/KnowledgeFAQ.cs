using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Help_Desk_2.Models;
using Help_Desk_2.DataAccessLayer;
using System.ComponentModel;

namespace Help_Desk_2.Models
{
    public class KnowledgeFAQ
    {
        public int ID { get; set; }

        [Display(Name = "Created on:"), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        [DataType(DataType.DateTime)]
        [Index]
        public DateTime dateComposed { get; set; }

        [Display(Name = "Expiries on:"), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        [DataType(DataType.DateTime)]
        public DateTime? expiryDate { get; set; }

        [Display(Name = "Submitted on:"), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        [DataType(DataType.DateTime)]
        public DateTime? dateSubmitted { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? dateUnpublished { get; set; }

        [Required]
        [Index]
        [Display(Name = "Brief description")]
        [StringLength(150, ErrorMessage = "Your brief description exceeds the maximum characters allowed")]
        public string headerText { get; set; }

        [Required]
        [AllowHtml]
        [Display(Name = "Detailed description")]
        [DataType(DataType.MultilineText)]
        public string description { get; set; }

        [AllowHtml]
        [Display(Name = "Notes")]
        [DataType(DataType.MultilineText)]
        public string notes { get; set; }

        [ForeignKey("Originator")]
        public Guid originatorID { get; set; } //AD Username

        [Display(Name = "Suggestion?")]
        [DefaultValue(false)]
        public bool suggest { get; set; }

        [Display(Name = "Published")]
        [DefaultValue(false)]
        public bool published { get; set; }

        [DefaultValue(false)]
        public bool archived { get; set; }

        [DefaultValue(false)]
        public bool deleted { get; set; }

        [DefaultValue(false)]
        public bool processed { get; set; }

        [DefaultValue(false)]
        public bool notifiedSubscriptions { get; set; }

        [Display(Name = "Links")]
        public string links { get; set; }

        [NotMapped]
        public string linkText { get; set; }

        [NotMapped]
        public string linkURL { get; set; }

        public virtual UserProfile Originator { get; set; }

        public virtual ICollection<Attachment> Files { get; set; }

        [NotMapped]     //List of files to delete
        public string deleteField { get; set; }

        public byte type { get; set; } //type => 1=FAQs, 2=Knowledge Base

        public int archiveID { get; set; }

        [NotMapped]
        [Display(Name = "Status")]
        public Statuses? status
        {
            get
            {

                if (deleted) return Statuses.Deleted;
                if (archived) return Statuses.Archived;
                if (published) return Statuses.Published;

                if (dateUnpublished != null)
                {
                    return Statuses.Unpublished;
                }
                else if (dateSubmitted != null)
                {
                    return Statuses.Submitted;
                }

                return Statuses.Draft;

            }

        }

        public virtual ICollection<WordList> wordList { get; set; }

        [NotMapped]
        [Display(Name = "Keywords", Prompt = "Select keywords from list")]
        public IEnumerable<WordList> keywords
        {
            get
            {
                return wordList.Where(x => x.type == 1 && !x.deleted);
            }
            set { keywords = value; }
        }

        [NotMapped]
        [Display(Name = "Expert Areas", Prompt = "Select expert areas from list")]
        public IEnumerable<WordList> expertAreas
        {
            get
            {
                return wordList.Where(x => x.type == 2 && !x.deleted);
            }
            set { expertAreas = value; }
        }


        public string GetArticleTypeString()
        {
            switch (type)
            {
                case 1:
                    return "FAQs";
                case 2:
                    return "KnowledgeBase";
                default:
                    throw new Exception($"Unexpected KnowledgeFAQ article type: {type}");
            }
        }

        //read-only faq approverID

    }
}