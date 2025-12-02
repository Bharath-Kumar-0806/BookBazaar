using System.ComponentModel.DataAnnotations;

namespace BookBazaarApi.ViewModels
{
    public class UpdateUserPasswordVM
    {
        [Required]
        public string? UserName { get; set; }


        [Required]
        public string? CurrentPassword { get; set; }


        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
               ErrorMessage = "Password must be at least 8 characters long and contain an uppercase letter, lowercase letter, number, and special character.")]
        public string? NewPassword { get; set; }


        [Required]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string? ConfirmNewPassword { get; set; }
    }
}
