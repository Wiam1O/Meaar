using System.ComponentModel.DataAnnotations;

namespace Meaar5.Models
{
    public class Admin
    {
        [Key]
        [Required]
        public int AdminId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}