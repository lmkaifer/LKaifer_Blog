using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LKaifer_Blog.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int BlogPostId { get; set; }
        public string AuthorId { get; set; }

        [AllowHtml]
        public string Body { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        
        public string UpdateReason { get; set; }              
        
        //Virtual Navigation section
        public virtual BlogPost BlogPost { get; set; }
        public virtual ApplicationUser Author { get; set; }

    }
}