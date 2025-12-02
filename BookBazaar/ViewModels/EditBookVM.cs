namespace BookBazaar.ViewModels
{
    public class EditBookVM
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Description { get; set; }
        public string? ISBN { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public IFormFile? Photo { get; set; }
        public string? PhotoPath { get; set; }
        public int CategoryId { get; set; }
    }
}
