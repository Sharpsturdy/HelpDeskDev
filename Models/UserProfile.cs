using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Help_Desk_2.Models
{
    public class UserProfile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid userID { get; set; }

        [MinLength(1)]
        public string loginName { get; set; }

        public string principalName { get; set; }

        [Display(Name = "First name")]
        [StringLength(50, ErrorMessage = "Field cannot exceed 50 characters")]
        public string firstName { get; set; }

        [Display(Name = "Last name")]
        [StringLength(50, ErrorMessage = "Field cannot exceed 50 characters")]
        public string surName { get; set; }

        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Enter a valid email address")]
        public string emailAddress { get; set; }

        [Display(Name = "Contact No.")]
        [StringLength(20, ErrorMessage = "Field cannot exceed 20 characters")]
        [DataType(DataType.PhoneNumber)]
        public string contactNumber { get; set; }

        [DefaultValue(false)]
        public bool isResponsible { get; set; }

        [DefaultValue(false)]
        public bool isFaqApprover { get; set; }

        [DefaultValue(false)]
        public bool isKbApprover { get; set; }

        [Display(Name = "Date of Last Login")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        [DataType(DataType.DateTime)]
        public DateTime? lastSignOn { get; set; }

        [DefaultValue(false)]
        public bool deleted { get; set; }

        //public int preferedLanguage { get; set;  }

        [NotMapped]
        [Display(Name = "Created by")]
        public string displayName { get { return firstName + " " + surName; } }

        //FAQ Substriptions
        [NotMapped]
        [Display(Name = "Keywords", Prompt = "Select keywords from list")]
        public IEnumerable<WordList> faqKeywords
        {
            get
            {
                return faqsubs.Where(x => x.type == 1);
            }
            set { faqKeywords = value; }
        }

        [NotMapped]
        [Display(Name = "Expert Areas", Prompt = "Select expert areas from list")]
        public IEnumerable<WordList> faqExpertAreas
        {
            get
            {
                return faqsubs.Where(x => x.type == 2);
            }
            set { faqExpertAreas = value; }
        }

        //KB Substriptions
        [NotMapped]
        [Display(Name = "Keywords", Prompt = "Select keywords from list")]
        public IEnumerable<WordList> kbKeywords
        {
            get
            {
                return kbsubs.Where(x => x.type == 1);
            }
            set { kbKeywords = value; }
        }

        [NotMapped]
        [Display(Name = "Expert Areas", Prompt = "Select expert areas from list")]
        public IEnumerable<WordList> kbExpertAreas
        {
            get
            {
                return kbsubs.Where(x => x.type == 2);
            }
            set { kbExpertAreas = value; }
        }

        public virtual ICollection<Ticket> tickets { get; set; }

        public virtual ICollection<WordList> faqsubs { get; set;  }

        public virtual ICollection<WordList> kbsubs { get; set; }
    }
}