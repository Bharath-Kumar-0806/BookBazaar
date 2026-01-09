using BookBazaarApi.DTOs;
using BookBazaarApi.Models;
using BookBazaarApi.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookBazaarApi.Repos.Interfaces
{
    public interface IOrderRepository
    {
        Task<int> PlaceOrderAsync(OrderRequestDto orderDto);
        Task<List<OrderSummaryVM>> GetMyOrdersAsync(string userId);
        Task<List<OrderSummaryVM>> GetFilteredOrdersAsync(string userId, int statusId);
        Task<BuyNowViewModel> GetBuyNowViewModel(string userId, int bookId);
        Task<int> ConfirmBuyNowAsync(BuyNowDTO orderDto);
        Task<bool> UpdateOrderAsync(Order order);
        Task<bool> DeleteOrderAsync(int id);
        Task<IEnumerable<Order>> GetAllAsync();
        Task<Order> GetByIdAsync(int id);
        Task AddAsync(Order order);
    }
}
