using System;
using System.ComponentModel.DataAnnotations;

namespace NewsSite.Models
{
    public class Comment
    {
        public int Id { get; set; }
        [Required]
        public News News { get; set; }

        [Required]
        public User User { get; set; }

        [Required]
        public DateTime Time{ get; set; }
        [Required]
        [MaxLength(100)]
        public string Content { get; set; }
    }
}
