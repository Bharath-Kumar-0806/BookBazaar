using BookBazaarApi.Models;
using BookBazaarApi.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookBazaarApi.Repos.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserByUsernameAsync(string username);
        Task<UserProfileVM> GetUserProfileAsync(string username);
        Task<bool> UpdateUserProfileAsync(UpdateUserProfileVM user);
        Task<bool> UpdateUserPasswordAsync(UpdateUserPasswordVM user);
        Task<int> AddAddressAsync(AddressViewModel address);
        Task<List<int>> GetWishListAsync(string username);
        Task<int> GetWishListCountAsync(string username);
        Task<int> AddToWishListAsync(string username, int bookId);
        Task<List<BookVM>> GetWishListBooksAsync(string username);
        Task<bool> RemoveFromWishListAsync(string username, int bookId);
    }
}
