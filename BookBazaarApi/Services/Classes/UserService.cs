using BookBazaarApi.Repos.Interfaces;
using BookBazaarApi.Services.Interfaces;
using BookBazaarApi.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookBazaarApi.Services.Classes
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserProfileVM> GetUserProfileAsync(string username)
        {
            return await _userRepository.GetUserProfileAsync(username);
        }

        public async Task<bool> UpdateUserProfileAsync(UpdateUserProfileVM user)
        {
            return await _userRepository.UpdateUserProfileAsync(user);
        }

        public async Task<bool> UpdateUserPasswordAsync(UpdateUserPasswordVM user)
        {
            return await _userRepository.UpdateUserPasswordAsync(user);
        }

        public async Task<int> AddAddressAsync(AddressViewModel address)
        {
            return await _userRepository.AddAddressAsync(address);
        }

        public async Task<List<int>> GetWishListAsync(string username)
        {
            return await _userRepository.GetWishListAsync(username);
        }

        public async Task<int> GetWishListCountAsync(string username)
        {
            return await _userRepository.GetWishListCountAsync(username);
        }

        public async Task<int> AddToWishListAsync(string username, int bookId)
        {
            return await _userRepository.AddToWishListAsync(username, bookId);
        }

        public async Task<List<BookVM>> GetWishListBooksAsync(string username)
        {
            return await _userRepository.GetWishListBooksAsync(username);
        }

        public async Task<bool> RemoveFromWishListAsync(string username, int bookId)
        {
            return await _userRepository.RemoveFromWishListAsync(username, bookId);
        }
    }
}
