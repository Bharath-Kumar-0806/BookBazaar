using BookBazaarApi.DAL;
using BookBazaarApi.Models;
using BookBazaarApi.Repos.Interfaces;
using BookBazaarApi.Services.Classes;
using BookBazaarApi.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookBazaarApi.Repos.Classes
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasherService _passwordHasher;

        public UserRepository(AppDbContext context, PasswordHasherService passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<UserProfileVM> GetUserProfileAsync(string username)
        {
            var user = await GetUserByUsernameAsync(username);
            if (user == null)
                return null;

            return new UserProfileVM
            {
                UserName = user.Username,
                Email = user.Email,
            };
        }

        public async Task<bool> UpdateUserProfileAsync(UpdateUserProfileVM user)
        {
            var existingUser = await GetUserByUsernameAsync(user.OldUserName);
            if (existingUser == null)
                return false;

            if (user.NewUserName != null)
            {
                if (!string.Equals(user.NewUserName, existingUser.Username, StringComparison.OrdinalIgnoreCase))
                {
                    if (await _context.Users.AnyAsync(u => u.Username == user.NewUserName))
                        throw new Exception("Username already exists.");
                    existingUser.Username = user.NewUserName;
                }
            }
            if (user.Email != null)
            {
                if (!string.Equals(user.Email, existingUser.Email, StringComparison.OrdinalIgnoreCase))
                {
                    if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                        throw new Exception("Email already registered.");
                    existingUser.Email = user.Email;
                }
            }
            existingUser.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateUserPasswordAsync(UpdateUserPasswordVM user)
        {
            var existingUser = await GetUserByUsernameAsync(user.UserName);
            if (existingUser == null)
                return false;

            if (!string.IsNullOrWhiteSpace(user.NewPassword))
            {
                if (!_passwordHasher.VerifyPassword(existingUser, user.CurrentPassword))
                {
                    throw new Exception("Invalid username or password.");
                }

                existingUser.PasswordHash = _passwordHasher.HashPassword(existingUser, user.NewPassword);
            }
            else
            {
                throw new Exception("New password cannot be empty.");
            }

            existingUser.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> AddAddressAsync(AddressViewModel address)
        {
            var user = await GetUserByUsernameAsync(address.UserName);
            if (user == null)
                return 0;

            var saveAddress = new Address
            {
                UserId = user.Id,
                Full_Name = address.Full_Name,
                Country = address.Country,
                State = address.State,
                City = address.City,
                Street = address.Street,
                House_No = address.House_No,
                Phone = address.Phone,
                Saved_Address = address.SaveNewAddress
            };
            _context.Addresses.Add(saveAddress);
            await _context.SaveChangesAsync();
            return saveAddress.Id;
        }

        public async Task<List<int>> GetWishListAsync(string username)
        {
            var user = await GetUserByUsernameAsync(username);
            if (user == null)
                return null;

            return await _context.FavoriteBooks
                .Where(f => f.UserId == user.Id)
                .Select(f => f.BookId)
                .ToListAsync();
        }

        public async Task<int> GetWishListCountAsync(string username)
        {
            var user = await GetUserByUsernameAsync(username);
            if (user == null)
                return -1;

            return await _context.FavoriteBooks.CountAsync(f => f.UserId == user.Id);
        }

        public async Task<int> AddToWishListAsync(string username, int bookId)
        {
            var user = await GetUserByUsernameAsync(username);
            if (user == null)
                throw new Exception("User not found.");

            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
                throw new Exception("Book not found.");

            var wishlist = await _context.FavoriteBooks.FirstOrDefaultAsync(f => f.UserId == user.Id && f.BookId == bookId);
            if (wishlist != null)
                throw new Exception("Already added to wish list.");

            var newWishListItem = new FavoriteBook
            {
                UserId = user.Id,
                BookId = book.Id
            };
            _context.FavoriteBooks.Add(newWishListItem);
            await _context.SaveChangesAsync();
            return newWishListItem.Id;
        }

        public async Task<List<BookVM>> GetWishListBooksAsync(string username)
        {
            var user = await GetUserByUsernameAsync(username);
            if (user == null)
                return null;

            var favoriteBookIds = await _context.FavoriteBooks
                .Where(f => f.UserId == user.Id)
                .Select(f => f.BookId)
                .ToListAsync();

            var books = await _context.Books
                .Where(b => favoriteBookIds.Contains(b.Id))
                .Select(b => new BookVM
                {
                    Id = b.Id,
                    Title = b.Title ?? string.Empty,
                    Author = b.Author ?? string.Empty,
                    Price = b.Price,
                    Stock = b.Stock,
                    PhotoPath = b.PhotoPath ?? string.Empty
                }).ToListAsync();
            return books;
        }

        public async Task<bool> RemoveFromWishListAsync(string username, int bookId)
        {
            var user = await GetUserByUsernameAsync(username);
            if (user == null)
                throw new Exception("User not found.");

            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
                throw new Exception("Book not found.");

            var favorite = await _context.FavoriteBooks.FirstOrDefaultAsync(f => f.UserId == user.Id && f.BookId == bookId);
            if (favorite == null)
                throw new Exception("Book is not in the wish list.");

            _context.FavoriteBooks.Remove(favorite);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
