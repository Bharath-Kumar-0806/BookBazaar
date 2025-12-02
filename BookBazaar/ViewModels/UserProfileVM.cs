using System.ComponentModel.DataAnnotations;

namespace BookBazaar.ViewModels
{
    public class UserProfileVM
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
