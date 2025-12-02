using BookBazaar.Models;

namespace BookBazaar.ViewModels
{
    public class AdminDashboardVM
    {
        public int TotalBooks { get; set; }
        public int TotalCategories { get; set; }
        public int TotalUsers { get; set; }
        public List<Books> RecentBooks { get; set; }
    }
}
