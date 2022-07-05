using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NewsSite.Models
{
    public class Tag
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
