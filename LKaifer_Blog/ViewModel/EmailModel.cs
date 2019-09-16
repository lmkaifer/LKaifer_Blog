using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LKaifer_Blog.ViewModel
{
    public class EmailModel
    {
        [Required, Display(Name = "Name")]
        [MaxLength(40), MinLength(1)]
        public string FromName { get; set; }

        [MaxLength(40), MinLength(1)]
        [Required, Display(Name = "Email"), EmailAddress]
        public string FromEmail { get; set; }

        [MaxLength(100), MinLength(1)]
        [Required]
        public string Subject { get; set; }


        [Required]
        [MinLength(1)]
        [AllowHtml]
        public string Body { get; set; }

    }
}