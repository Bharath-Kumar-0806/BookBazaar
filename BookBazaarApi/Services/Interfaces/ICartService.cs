using BookBazaarApi.Models;
using BookBazaarApi.ViewModels;

namespace BookBazaarApi.Services.Interfaces
{
    public interface ICartService
    {
        Task<CartViewModel> GetCartAsync(string userId);
        Task<Cart> AddToCartAsync(string userId, int bookId, int quantity);
        Task<bool> RemoveFromCartAsync(string userId, int cartItemId);
        Task<bool> UpdateCartItemAsync(string userId, int cartItemId, int quantity);
        Task<bool> ClearCartAsync(string userId);
        Task<int> GetCartItemCountAsync(string userName);
    }
}
