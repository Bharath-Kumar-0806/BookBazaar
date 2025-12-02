namespace BookBazaar.ViewModels
{
    public class OrderItemVM
    {
        public string PhotoPath { get; set; } = string.Empty;
        public string BookTitle { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

    }
}
