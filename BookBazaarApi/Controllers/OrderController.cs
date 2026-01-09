using BookBazaarApi.DTOs;
using BookBazaarApi.Helpers;
using BookBazaarApi.Services.Interfaces;
using BookBazaarApi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookBazaarApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("PlaceOrder")]
        public async Task<IActionResult> PlaceOrder(OrderRequestDto orderDto)
        {
            try
            {
                var orderId = await _orderService.PlaceOrderAsync(orderDto);
                var response = new ResponseModel<object>
                {
                    Success = true,
                    Result = orderId,
                    Message = "Order Placed Successfully"
                };
                return Ok(response);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetMyOrders")]
        public async Task<IActionResult> GetMyOrders(RequestModel data)
        {
            var result = await _orderService.GetMyOrdersAsync(data.Key);
            if (result == null)
            {
                return Unauthorized(new ResponseModel<List<OrderSummaryVM>>
                {
                    Success = false,
                    Message = "User not found."
                });
            }
            return Ok(new ResponseModel<List<OrderSummaryVM>>
            {
                Success = true,
                Message = "Orders fetched successfully.",
                Result = result
            });
        }

        [HttpPost("GetFilterdOrders")]
        public async Task<IActionResult> GetFilterdOrders(RequestModel data)
        {
            var result = await _orderService.GetFilteredOrdersAsync(data.Key, data.Id);
            if (result == null)
            {
                return Unauthorized(new ResponseModel<List<OrderSummaryVM>>
                {
                    Success = false,
                    Message = "User not found."
                });
            }
            return Ok(new ResponseModel<List<OrderSummaryVM>>
            {
                Success = true,
                Message = data.Id.ToString(),
                Result = result
            });
        }

        [HttpPost("BuyNow")]
        public async Task<IActionResult> BuyNow(RequestModel data)
        {
            try
            {
                var viewModel = await _orderService.GetBuyNowViewModel(data.Key, data.Id);
                var response = new ResponseModel<BuyNowViewModel>
                {
                    Success = true,
                    Result = viewModel,
                    Message = "Book Fetched"
                };
                return Ok(response);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("ConfirmBuyNow")]
        public async Task<IActionResult> ConfirmBuyNow(BuyNowDTO orderDto)
        {
            try
            {
                var orderId = await _orderService.ConfirmBuyNowAsync(orderDto);
                var response = new ResponseModel<object>
                {
                    Success = true,
                    Result = orderId,
                    Message = "Order Placed Successfully"
                };
                return Ok(response);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

