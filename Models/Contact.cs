using System.ComponentModel.DataAnnotations;

namespace NewsSite.Models
{
    public class Contact
    {
        public int Id { get; set; }

        [Required]
        [MinLength(3)]
        public string Name { get; set; }
        [Required]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$")]
        public string Email { get; set; }
        [Required]
        [MinLength(3)]
        public string Subject { get; set; }

        [Required]
        [StringLength(maximumLength: 2000, MinimumLength = 3)]
        public string Message { get; set; }

    }
}
