using System;
using System.Collections.Generic;
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
        public Guid userID { get; set;  }

        [MinLength(1)]
        public string loginName { get; set;  }

        public string principalName { get; set;  }

        [Display(Name = "First name")]
        [StringLength(50, ErrorMessage = "Field cannot exceed 50 characters")]
        public string firstName { get; set;  }

        [Display(Name = "Last name")]
        [StringLength(50, ErrorMessage = "Field cannot exceed 50 characters")]
        public string surName { get; set;  }

        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress,ErrorMessage = "Enter a valid email address")]
        public string emailAddress { get; set;  }

        [Display(Name = "Contact No.")]
        [StringLength(20, ErrorMessage = "Field cannot exceed 20 characters")]
        [DataType(DataType.PhoneNumber)]
        public string contactNumber { get; set;  }

        //public int preferedLanguage { get; set;  }

        [NotMapped]
        public string displayName { get { return firstName + " " + surName; } }
    }
}