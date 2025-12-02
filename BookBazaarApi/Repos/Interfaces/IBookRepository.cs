using BookBazaarApi.Helpers;
using BookBazaarApi.Models;
using BookBazaarApi.ViewModels;

namespace BookBazaarApi.Repos.Interfaces
{
    public interface IBookRepository
    {
        //admin
        Task<List<Book>> GetAllAsync();
        Task<List<Book>> SearchBook(RequestModel data);
        Task<Book> GetByIdAsync(int id);
        Task<bool> AddAsync(Book book);
        Task UpdateAsync(Book book);
        Task DeleteAsync(int id);


        //user
        Task<UserHomeVM> HomePageBooksAsync(RequestModel data);
        Task<List<BookVM>> GetNewArrivalsViewAllAsync(RequestModel data);
        Task<List<BookVM>> GetBestSellersViewAllAsync(RequestModel data);
        Task<BookDetailsVM> GetBookDetailsAsync(int id);
        Task<List<BookVM>> GetBooksByCategoryId(RequestModel data);
        Task<List<BookVM>> SearchResults(RequestModel data);
    }
}
