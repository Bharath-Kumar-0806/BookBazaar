using BookBazaarApi.Models;

namespace BookBazaarApi.ViewModels
{
    public class AdminDashboardVM
    {
        public int TotalBooks { get; set; }
        public int TotalCategories { get; set; }
        public int TotalUsers { get; set; }
        public List<Book> RecentBooks { get; set; }
    }
}
