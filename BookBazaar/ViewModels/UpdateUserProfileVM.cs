using System.ComponentModel.DataAnnotations;

namespace BookBazaar.ViewModels
{
    public class UpdateUserProfileVM
    {
        public string? OldUserName { get; set; }
        public string? NewUserName { get; set; }

        [EmailAddress]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid email.")]
        public string? Email { get; set; }

    }
}
