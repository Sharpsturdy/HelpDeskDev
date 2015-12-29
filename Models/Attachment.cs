using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Web;

namespace Help_Desk_2.Models
{
    public class Attachment
    {

        [Key]
        public Int32 ID { get; set; }

        [ForeignKey("Ticket")]
        public int parentID { get; set; }

        [Required]
        [Display(Name = "File Name")]
        public string fileName { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        [Display(Name = "Browse to select a file")]
        public string filePath { get; set; }

        public virtual Ticket Ticket { get; set; }

        [NotMapped]
        public long size
        {
            get
            {
                FileInfo fi = new FileInfo(filePath);
                return fi.Length;
            }
        }

        [NotMapped]
        public string displaySize
        {
            //
            get
            {
                long mb = 1024 * 1024;
                long s = size; //compute size once

                if(s < mb)
                {
                    return String.Format("{0,6:0.00} kb", (s / 1024));
                } else
                {
                    return String.Format("{0,6:0.00} mb", (s / mb));
                }
                
            }
        }
    }
}