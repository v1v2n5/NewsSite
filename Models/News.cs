using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace NewsSite.Models
{
    public class News
    {
        
        public int Id {get; set; }
        [Required]
        public DateTime Time { get; set; }

        public User Author { get; set; }

        [Required]
        public string Title { get; set; }

        public string PreviewImage { get; set; }
        
        [Required]
        public string PreviewContent { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public Category Category { get; set; }

        public IEnumerable<NewsTags> Tags { get; set; }

        public IEnumerable<Comment> Comments { get; set; }

    }

    public class NewsTags
    {
        public int Id{ get; set; }

        [Required]
        public Tag Tag { get; set; }
    }
}
