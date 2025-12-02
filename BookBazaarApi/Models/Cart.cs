using System.ComponentModel.DataAnnotations;

namespace BookBazaarApi.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }

        public string? UserName { get; set; }  // Nullable for guest users

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        // Navigation property
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
