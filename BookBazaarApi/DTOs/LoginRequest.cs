using System.ComponentModel.DataAnnotations;

namespace BookBazaarApi.DTOs
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid email.")]
        public string? Email { get; set; }
        [Required]
        
        public string? Password { get; set; }
    }
}
