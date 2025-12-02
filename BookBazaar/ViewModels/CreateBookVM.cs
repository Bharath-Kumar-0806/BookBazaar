using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookBazaar.ViewModels
{
    public class CreateBookVM
    {
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Description { get; set; }
        public string? ISBN { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public IFormFile? Photo { get; set; }
        public int CategoryId { get; set; }
        //public string?  CategoryName { get; set; }
        //public List<SelectListItem>? CategoryList { get; set; }
    }
}
