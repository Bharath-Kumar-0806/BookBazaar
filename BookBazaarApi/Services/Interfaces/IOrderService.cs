using BookBazaarApi.DTOs;
using BookBazaarApi.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookBazaarApi.Services.Interfaces
{
    public interface IOrderService
    {
        Task<int> PlaceOrderAsync(OrderRequestDto orderDto);
        Task<List<OrderSummaryVM>> GetMyOrdersAsync(string userId);
        Task<List<OrderSummaryVM>> GetFilteredOrdersAsync(string userId, int statusId);
        Task<BuyNowViewModel> GetBuyNowViewModel(string userId, int bookId);
        Task<int> ConfirmBuyNowAsync(BuyNowDTO orderDto);
    }
}
