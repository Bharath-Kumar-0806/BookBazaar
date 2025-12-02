using System.ComponentModel.DataAnnotations;

namespace BookBazaar.ViewModels
{
    public class AdminProfileManagingVM
    {
        public string Username { get; set; } = string.Empty;
        [Required]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid email.")]
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = "Admin";
        public DateTime CreatedAt { get; set; }
    }
}
