using BookBazaarApi.DAL;
using BookBazaarApi.Helpers;
using BookBazaarApi.Models;
using BookBazaarApi.Repos.Interfaces;
using BookBazaarApi.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BookBazaarApi.Repos.Classes
{

    public class BookRepository : IBookRepository
    {
        private readonly AppDbContext _dal;

        public BookRepository(AppDbContext appDbContext)
        {
            _dal = appDbContext;
        }

        public async Task<List<Book>> GetAllAsync() =>
            await _dal.Books.ToListAsync();

        public async Task<List<Book>> SearchBook(RequestModel model)
        {
            string search = model.Key.Trim().ToLower();

            var books = await _dal.Books
                               .Where(b => b.Title.ToLower().Contains(search) ||
                                        b.ISBN.ToLower().Contains(search) ||
                                        b.Author.ToLower().Contains(search)
                               )
                               .ToListAsync();
            return books;
        }

        public async Task<Book?> GetByIdAsync(int id) =>
            await _dal.Books.FirstOrDefaultAsync(x => x.Id == id);

        public async Task<bool> AddAsync(Book book)
        {
            try
            {
                _dal.Add(book);
                _dal.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task UpdateAsync(Book book)
        {
            using var tra = _dal.Database.BeginTransaction();
            try
            {
                _dal.Books.Update(book);
                await _dal.SaveChangesAsync();

                await tra.CommitAsync();
            }
            catch(Exception)
            {
                tra.Rollback();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var book = await _dal.Books.FindAsync(id);
            if (book != null)
            {
                _dal.Books.Remove(book);
                await _dal.SaveChangesAsync();
            }
        }

        //public async Task<UserHomeVM> GetNewArrivalsAsync(RequestModel model)
        //{
        //    var newArrivalsBooks = await _dal.Books
        //                            .OrderByDescending(b => b.Id)
        //                            .Take(10)
        //                            .ToListAsync();

        //    var bestSellerBooks = await _dal.OrderItems
        //                                 .GroupBy(o => o.BookId)
        //                                 .OrderByDescending(g => g.Count())
        //                                 .Take(10)
        //                                 .Select(g => g.Key) // BookId
        //                                 .ToListAsync();

        //    if (model.Key !=null)
        //    {
        //        // Step 1: Get wishlist book IDs if user is logged in
        //        List<int> wishListBookIds = new();
        //        var user = await _dal.Users.FirstOrDefaultAsync(u => u.Username == model.Key);
        //        if (user != null)
        //        {
        //            wishListBookIds = await _dal.FavoriteBooks
        //                .Where(f => f.UserId == user.Id)
        //                .Select(f => f.BookId)
        //                .ToListAsync();
        //        }
        //        // Step 2: Map books to HomeBooksVM with wishlist flag
        //        var nab = newArrivalsBooks.Select(b => new BookVM
        //        {
        //            Id = b.Id,
        //            Title = b.Title ?? string.Empty,
        //            Author = b.Author ?? string.Empty,
        //            Price = b.Price,
        //            Stock = b.Stock,
        //            PhotoPath = b.PhotoPath ?? string.Empty,
        //            IsInWishList = wishListBookIds.Contains(b.Id)
        //        }).ToList();
        //        var bsb = bestSellerBooks.Select(b => new BookVM
        //        {
        //            Id = b.Id,
        //            Title = b.Title ?? string.Empty,
        //            Author = b.Author ?? string.Empty,
        //            Price = b.Price,
        //            Stock = b.Stock,
        //            PhotoPath = b.PhotoPath ?? string.Empty,
        //            IsInWishList = wishListBookIds.Contains(b.Id)
        //        }).ToList();
        //        var userdata = new UserHomeVM
        //        {
        //            NewArrivals = nab,
        //            BestSellers = bsb,
        //        };
        //        return userdata;
        //    }

        //    var newArrivals = newArrivalsBooks.Select(b => new BookVM
        //    {
        //        Id = b.Id,
        //        Title = b.Title ?? string.Empty,
        //        Author = b.Author ?? string.Empty,
        //        Price = b.Price,
        //        Stock=b.Stock,
        //        PhotoPath = b.PhotoPath ?? string.Empty,
        //        IsInWishList=false
        //    }).ToList();


        //    var bestSellers = await _dal.Books
        //                        .Where(b => bestSellerBooks.Contains(b.Id))
        //                        .Select(b => new BookVM
        //                        {
        //                                Id = b.Id,
        //                                Title = b.Title,
        //                                Author = b.Author,
        //                                PhotoPath = b.PhotoPath,
        //                                Price = b.Price,
        //                                Stock = b.Stock,
        //                                IsInWishList = false
        //                        }).ToListAsync();
        //    var data = new UserHomeVM
        //    {
        //        NewArrivals = newArrivals,
        //        BestSellers = bestSellers,
        //    };
        //    return data;

        //}
        public async Task<UserHomeVM> HomePageBooksAsync(RequestModel model)
        {
            // Step 1: Fetch new arrivals (latest books)
            var newArrivalsBooks = await _dal.Books
                .OrderByDescending(b => b.Id)
                .Take(10)
                .ToListAsync();

            // Step 2: Get best seller book IDs from OrderItems
            var bestSellerBookIds = await _dal.OrderItems
                .GroupBy(o => o.BookId)
                .OrderByDescending(g => g.Count())
                .Take(10)
                .Select(g => g.Key)
                .ToListAsync();

            // Step 3: Get full Book entities for best sellers
            var bestSellerBooks = await _dal.Books
                .Where(b => bestSellerBookIds.Contains(b.Id))
                .ToListAsync();

            // Step 4: Get wishlist if user is logged in
            List<int> wishListBookIds = new();
            if (!string.IsNullOrEmpty(model?.Key))
            {
                var user = await _dal.Users.FirstOrDefaultAsync(u => u.Username == model.Key);
                if (user != null)
                {
                    wishListBookIds = await _dal.FavoriteBooks
                        .Where(f => f.UserId == user.Id)
                        .Select(f => f.BookId)
                        .ToListAsync();
                }
            }

            // Step 5: Map books to BookVMs
            var newArrivals = newArrivalsBooks
                .Select(b => MapToBookVM(b, wishListBookIds))
                .ToList();

            var bestSellers = bestSellerBooks
                .Select(b => MapToBookVM(b, wishListBookIds))
                .ToList();

            // Step 6: Return data
            return new UserHomeVM
            {
                NewArrivals = newArrivals,
                BestSellers = bestSellers
            };
        }

        // Helper method to map Book to BookVM
        private BookVM MapToBookVM(Book b, List<int> wishListBookIds)
        {
            return new BookVM
            {
                Id = b.Id,
                Title = b.Title ?? string.Empty,
                Author = b.Author ?? string.Empty,
                Price = b.Price,
                Stock = b.Stock,
                PhotoPath = b.PhotoPath ?? string.Empty,
                IsInWishList = wishListBookIds.Contains(b.Id)
            };
        }

        public async Task<List<BookVM>> GetNewArrivalsViewAllAsync(RequestModel model)
        {
            var recentBooks = await _dal.Books
                                .OrderByDescending(b => b.Id)
                                .ToListAsync();

            if (model.Key != null)
            {
                // Step 1: Get wishlist book IDs if user is logged in
                List<int> wishListBookIds = new();
                var user = await _dal.Users.FirstOrDefaultAsync(u => u.Username == model.Key);
                if (user != null)
                {
                    wishListBookIds = await _dal.FavoriteBooks
                        .Where(f => f.UserId == user.Id)
                        .Select(f => f.BookId)
                        .ToListAsync();
                }
                // Step 2: Map books to HomeBooksVM with wishlist flag
                var userdata = recentBooks.Select(b => new BookVM
                {
                    Id = b.Id,
                    Title = b.Title ?? string.Empty,
                    Author = b.Author ?? string.Empty,
                    Price = b.Price,
                    Stock = b.Stock,
                    PhotoPath = b.PhotoPath ?? string.Empty,
                    IsInWishList = wishListBookIds.Contains(b.Id)
                }).ToList();

                return userdata;
            }

            var data = recentBooks.Select(b => new BookVM
            {
                Id = b.Id,
                Title = b.Title ?? string.Empty,
                Author = b.Author ?? string.Empty,
                Price = b.Price,
                Stock = b.Stock,
                PhotoPath = b.PhotoPath ?? string.Empty,
                IsInWishList = false
            }).ToList();

            return data;

        }
        public async Task<List<BookVM>> GetBestSellersViewAllAsync(RequestModel model)
        {
            // Step 1: Get best seller book IDs from OrderItems
            var bestSellerBookIds = await _dal.OrderItems
                .GroupBy(o => o.BookId)
                .OrderByDescending(g => g.Count())
                .Take(10)
                .Select(g => g.Key)
                .ToListAsync();

            // Step 2: Get full Book entities for best sellers
            var bestSellerBooks = await _dal.Books
                .Where(b => bestSellerBookIds.Contains(b.Id))
                .ToListAsync();

            // Step 3: Get wishlist if user is logged in
            List<int> wishListBookIds = new();
            if (!string.IsNullOrEmpty(model?.Key))
            {
                var user = await _dal.Users.FirstOrDefaultAsync(u => u.Username == model.Key);
                if (user != null)
                {
                    wishListBookIds = await _dal.FavoriteBooks
                        .Where(f => f.UserId == user.Id)
                        .Select(f => f.BookId)
                        .ToListAsync();
                }
            }

            var bestSellers = bestSellerBooks
                .Select(b => MapToBookVM(b, wishListBookIds))
                .ToList();

            return bestSellers;

        }

        public async Task<BookDetailsVM> GetBookDetailsAsync(int id)
        {

            var book = await _dal.Books.FirstOrDefaultAsync(x => x.Id == id);

            if (book == null)
                return null; // or throw a NotFoundException / custom error

            var category = await _dal.Categories.FirstOrDefaultAsync(c => c.Id == book.CategoryId);
            var data = new BookDetailsVM
            {
                Id = book.Id,
                Title = book.Title ?? string.Empty,
                Author = book.Author ?? string.Empty,
                Description=book.Description ?? string.Empty,
                ISBN=book.ISBN ?? string.Empty,
                Price = book.Price,
                Stock=book.Stock,
                PhotoPath = book.PhotoPath ?? string.Empty,
                CategoryName = category?.Name ?? "Unknown"
            };
            return data;
        }


        public async Task<List<BookVM>> GetBooksByCategoryId(RequestModel model)
        {

            var category = await _dal.Categories.FindAsync(model.Id);
            if (category == null) return null;


            var books = await _dal.Books
                .Where(b => b.CategoryId == model.Id)
                .ToListAsync();

            if (model.Key != null)
            {
                // Step 1: Get wishlist book IDs if user is logged in
                List<int> wishListBookIds = new();
                var user = await _dal.Users.FirstOrDefaultAsync(u => u.Username == model.Key);
                if (user != null)
                {
                    wishListBookIds = await _dal.FavoriteBooks
                        .Where(f => f.UserId == user.Id)
                        .Select(f => f.BookId)
                        .ToListAsync();
                }
                // Step 2: Map books to HomeBooksVM with wishlist flag
                var userdata = books.Select(b => new BookVM
                {
                    Id = b.Id,
                    Title = b.Title ?? string.Empty,
                    Author = b.Author ?? string.Empty,
                    Price = b.Price,
                    Stock = b.Stock,
                    PhotoPath = b.PhotoPath ?? string.Empty,
                    IsInWishList = wishListBookIds.Contains(b.Id)
                }).ToList();

                return userdata;
            }

            var data = books.Select(b => new BookVM
            {
                Id = b.Id,
                Title = b.Title ?? string.Empty,
                Author = b.Author ?? string.Empty,
                Price = b.Price,
                Stock = b.Stock,
                PhotoPath = b.PhotoPath ?? string.Empty,
                IsInWishList=false
            }).ToList();

            return data;

        }

        public async Task<List<BookVM>> SearchResults(RequestModel model)
        {
            string search = model.Key.Trim().ToLower();

            var books = await _dal.Books
                               .Where(b =>b.Title.ToLower().Contains(search) ||
                                        b.ISBN.ToLower().Contains(search) ||
                                        b.Author.ToLower().Contains(search)
                               )
                               .ToListAsync();
            var data = books.Select(b => new BookVM
            {
                Id = b.Id,
                Title = b.Title ?? string.Empty,
                Author = b.Author ?? string.Empty,
                Price = b.Price,
                Stock = b.Stock,
                PhotoPath = b.PhotoPath ?? string.Empty,
                IsInWishList = false
            }).ToList();

            return data;
        }

      
    }
}
