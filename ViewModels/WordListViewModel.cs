using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Help_Desk_2.ViewModels
{
    public class WordListViewModel
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(30)]
        public string text { get; set; }

        [Required]
        public int type { get; set; } //type => 1=Keyword, 2=ExpertArea,3= Title,etc
    }
}