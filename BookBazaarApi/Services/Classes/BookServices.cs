using BookBazaarApi.Helpers;
using BookBazaarApi.Models;
using BookBazaarApi.Repos.Interfaces;
using BookBazaarApi.Services.Interfaces;
using BookBazaarApi.ViewModels;

namespace BookBazaarApi.Services.Classes
{
    public class BookServices: IBookService
    {
        private readonly IBookRepository _bookRepository;

        public BookServices(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<List<Book>> GetAllBooksAsync()
        {
            return await _bookRepository.GetAllAsync();
        }
        public async Task<List<Book>> SearchBookAsync(RequestModel data)
        {
            return await _bookRepository.SearchBook(data);
        }

        public async Task<Book> GetBookByIdAsync(int id)
        {
            return await _bookRepository.GetByIdAsync(id);
        }

        public async Task<bool> CreateBookAsync(Book book)
        {
                return await _bookRepository.AddAsync(book);
        }

        public async Task<bool> UpdateBookAsync(Book book)
        {
            try
            {
                await _bookRepository.UpdateAsync(book);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null)
                return false;

            await _bookRepository.DeleteAsync(id);
            return true;
        }

        public async Task<UserHomeVM> HomePageBooks(RequestModel data)
        {
            return await _bookRepository.HomePageBooksAsync(data);
        }
        public async Task<List<BookVM>> GetNewArrivalsViewAll(RequestModel data)
        {
            return await _bookRepository.GetNewArrivalsViewAllAsync(data);
        }
        public async Task<List<BookVM>> GetBestSellersViewAll(RequestModel data)
        {
            return await _bookRepository.GetBestSellersViewAllAsync(data);
        }

        public async Task<BookDetailsVM> GetBookDetailsAsync(int id)
        {
            return await _bookRepository.GetBookDetailsAsync(id);
        }

        public async Task<List<BookVM>> GetBooksByCategoryId(RequestModel data)
        {
            return await _bookRepository.GetBooksByCategoryId(data);
        }
        public async Task<List<BookVM>> SearchResults(RequestModel data)
        {
            return await _bookRepository.SearchResults(data);
        }
    }
}
