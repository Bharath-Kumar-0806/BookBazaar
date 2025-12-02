namespace BookBazaar.ViewModels
{
    public class BookVM
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public int Stock { get; set; }
        public decimal Price { get; set; }
        public string PhotoPath { get; set; } = string.Empty;
        public bool IsInWishList { get; set; }
    }
}
