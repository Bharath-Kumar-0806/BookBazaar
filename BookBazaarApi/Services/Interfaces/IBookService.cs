using BookBazaarApi.Helpers;
using BookBazaarApi.Models;
using BookBazaarApi.ViewModels;

namespace BookBazaarApi.Services.Interfaces
{
    public interface IBookService
    {
        //admin services
        Task<List<Book>> GetAllBooksAsync();
        Task<List<Book>> SearchBookAsync(RequestModel data);
        Task<bool> CreateBookAsync(Book book);
        Task<bool> UpdateBookAsync(Book book);
        Task<bool> DeleteBookAsync(int id);
        Task<Book> GetBookByIdAsync(int id);

        //user services
        Task<List<BookVM>> SearchResults(RequestModel data);
        Task<UserHomeVM> HomePageBooks(RequestModel data);
        Task<List<BookVM>> GetNewArrivalsViewAll(RequestModel data);
        Task<List<BookVM>> GetBestSellersViewAll(RequestModel data);
        Task<BookDetailsVM> GetBookDetailsAsync(int id);
        Task<List<BookVM>> GetBooksByCategoryId(RequestModel data);
    }
}
