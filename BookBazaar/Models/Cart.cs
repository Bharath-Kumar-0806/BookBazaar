using System.ComponentModel.DataAnnotations;

namespace BookBazaar.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }

        public string? UserName { get; set; } 

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        // Navigation property
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
