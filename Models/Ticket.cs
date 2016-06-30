using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using System.DirectoryServices.AccountManagement;
using Help_Desk_2.Utilities;
using Help_Desk_2.DataAccessLayer;
using System.Web.Mvc;
using System.ComponentModel;

namespace Help_Desk_2.Models
{
    public enum Statuses
    {
        Suggestion, Draft, Submitted, Checked,Accepted,Rejected,Junked, Assigned, Returned, Unpblished, Published, OnHold, Completed, Archived, Expired, Deleted
    }

    public enum SanityChecks
    {
        Accept, Reject, Junk
    }
    public class Ticket
    {
        [Display(Name = "Temporary ticket No.")]
        public int ID { get; set;  }
        
        [Display(Name = "Ticket No.")]
        public int ticketID { get; set;  }

        [ForeignKey("Originator")]
        public Guid originatorID { get; set; } //AD Username


        public Guid? responsibleID { get; set; } //Link to Responsible person

        [Column(TypeName = "DateTime")]
        [Display(Name = "Created on:"), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        [DataType(DataType.DateTime)]
        public DateTime dateComposed { get; set; }

        [Column(TypeName = "DateTime")]
        [Display(Name = "Expires on:"), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        [DataType(DataType.DateTime)]
        public DateTime? expiryDate { get; set; }

        [Column(TypeName = "DateTime")]
        [Display(Name = "Date Completed"), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        [DataType(DataType.DateTime)]
        public DateTime? dateCompleted { get; set; }

        [DefaultValue(false)]
        public bool deleted { get; set; }

        [DefaultValue(false)]
        public bool onhold { get; set; }

        [DefaultValue(false)]
        public bool returned { get; set; }

        [NotMapped]
        public string message { get; set; }

        [Required]
        [Display(Name = "Brief description")]
        [StringLength(150, ErrorMessage = "Your brief description exceeds the maximum characters allowed")]
        public string headerText { get; set; }

        [Required]
        [AllowHtml]
        [Display(Name = "Detailed description")]
        [DataType(DataType.MultilineText)]
        public string description { get; set; }

        
        [Display(Name = "Links")]
        public string links { get; set; }

        [NotMapped]
        public string linkText { get; set; }

        [NotMapped]
        public string linkURL { get; set; }

        [Column(TypeName = "DateTime")]
        [DataType(DataType.DateTime)]
        public DateTime? dateSubmitted { get; set; }
        
        [Column(TypeName = "DateTime")]
        [DataType(DataType.DateTime), Display(Name = "Release Date"), DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? dateL1Release { get; set; }

        [Column(TypeName = "DateTime")]
        [DataType(DataType.DateTime), Display(Name = "Release Date"), DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? dateL2Release { get; set; }
                
        [NotMapped]
        [Display(Name = "Status")]
        public Statuses? status {
            get {
                if (deleted) return Statuses.Deleted;
                if (expiryDate <= DateTime.Now) return Statuses.Expired;
                if (onhold) { return Statuses.OnHold; };
                if (returned) { return Statuses.Returned; };

                if (dateSubmitted == null)  return Statuses.Draft ; 
                if (dateL1Release == null)  return Statuses.Submitted;
                if (dateL2Release == null) {
                    if (sanityCheck == SanityChecks.Accept)
                        return Statuses.Accepted;

                    if (sanityCheck == SanityChecks.Reject)
                        return Statuses.Rejected;

                    //else
                    return Statuses.Junked;
                }
                if (dateCompleted == null)  return Statuses.Assigned; 
                
                return Statuses.Completed; 
            }
            
        }

        [Display(Name = "Reason")]
        public string reason { get; set; }

        [Display(Name = "Admin Comments")]
        [DataType(DataType.MultilineText)]
        public string adminComments { get; set; }

        [NotMapped]
        [Display(Name = "Choose file")]
        public string filePath { get; set; }

        [NotMapped]     //List of files to delete
        public string deleteField { get; set; }

        [Display(Name = "Sanity Check")]
        public SanityChecks? sanityCheck { get; set; }

        [Display(Name = "Summary")]
        [StringLength(250, ErrorMessage = "Your completion summary exceeds the maximum characters allowed")]
        public string summary { get; set; }

        [Display(Name = "Report")]
        [DataType(DataType.MultilineText)]
        public string report { get; set; }

        
        [NotMapped]
        [Display(Name = "Keywords", Prompt = "Select keywords from list")]
        public IEnumerable<WordList> keywords
        {
            get
            {
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

        [NotMapped]
        public string  displayID {  get
            {
                if (dateL2Release !=null)
                    return string.Format("{0,5:D5}", ticketID);

                return string.Format("TMP{0,5:D5}", ID);
            }
        }

        public virtual ICollection<WordList> wordList { get; set; }

        public virtual UserProfile Originator { get; set; }

        [ForeignKey("responsibleID")]
        public virtual UserProfile Responsible { get; set; }

        public virtual ICollection<Attachment> Files { get; set; }
                
        public virtual ICollection<AuditTrail> AuditTrail { get; set; }

    }
}