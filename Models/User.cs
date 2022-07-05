using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace NewsSite.Models
{
    [Index(nameof(NickName),IsUnique = true)]
    public class User : IdentityUser
    {
        [Required]
        
        [MaxLength(100)]
        public string NickName { get; set; }

    }
}
