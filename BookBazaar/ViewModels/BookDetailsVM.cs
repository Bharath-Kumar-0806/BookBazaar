namespace BookBazaar.ViewModels
{
    public class BookDetailsVM
    {
            public int Id { get; set; }
            public string Title { get; set; } = string.Empty;
            public string Author { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string ISBN { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public int Stock { get; set; }
            public string PhotoPath { get; set; } = string.Empty;
            public string CategoryName { get; set; } = string.Empty;
    }
}
