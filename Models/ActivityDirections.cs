using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace NewsSite.Models
{
    public class ActivityDirections
    {
        
        public int Id {get; set; }

        [Required]
        public string Title { get; set; }

        public string PreviewImage { get; set; }
        
        [Required]
        public string PreviewContent { get; set; }

        [Required]
        public string Content { get; set; }

    }

}
