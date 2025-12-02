namespace BookBazaar.ViewModels
{
    public class CartItemViewModel
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public string Author { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string ImageUrl { get; set; }
        public decimal TotalPrice => UnitPrice * Quantity;
    }
}
