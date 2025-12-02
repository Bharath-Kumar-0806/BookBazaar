using System.ComponentModel.DataAnnotations;

namespace BookBazaarApi.ViewModels
{
    public class UserProfileVM
    {
        public string UserName { get; set; } = string.Empty;
        //[Required]
        //[RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid email.")]
        public string Email { get; set; } = string.Empty;
        //public DateTime RegisteredAt { get; set; }
    }
}
