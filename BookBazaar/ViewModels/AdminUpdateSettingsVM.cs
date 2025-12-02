using System.ComponentModel.DataAnnotations;

namespace BookBazaar.ViewModels
{
    public class AdminUpdateSettingsVM
    {
        [Required]
        public string FullName { get; set; } = string.Empty;
        [Required, EmailAddress]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid email.")]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "Password must be at least 8 characters long and contain an uppercase letter, lowercase letter, number, and special character.")]
        public string NewPassword { get; set; } = string.Empty;
    }
}
