using BookBazaarApi.DTOs;
using BookBazaarApi.Repos.Interfaces;
using BookBazaarApi.Services.Interfaces;
using BookBazaarApi.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookBazaarApi.Services.Classes
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<int> PlaceOrderAsync(OrderRequestDto orderDto)
        {
            return await _orderRepository.PlaceOrderAsync(orderDto);
        }

        public async Task<List<OrderSummaryVM>> GetMyOrdersAsync(string userId)
        {
            return await _orderRepository.GetMyOrdersAsync(userId);
        }

        public async Task<List<OrderSummaryVM>> GetFilteredOrdersAsync(string userId, int statusId)
        {
            return await _orderRepository.GetFilteredOrdersAsync(userId, statusId);
        }

        public async Task<BuyNowViewModel> GetBuyNowViewModel(string userId, int bookId)
        {
            return await _orderRepository.GetBuyNowViewModel(userId, bookId);
        }

        public async Task<int> ConfirmBuyNowAsync(BuyNowDTO orderDto)
        {
            return await _orderRepository.ConfirmBuyNowAsync(orderDto);
        }
    }
}
