namespace BookBazaarApi.ViewModels
{
    public class UserHomeVM
    {
        public List<BookVM> NewArrivals { get; set; } = new();
        public List<BookVM> BestSellers { get; set; } = new();
    }
}
