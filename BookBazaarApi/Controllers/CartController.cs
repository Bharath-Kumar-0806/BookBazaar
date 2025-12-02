using BookBazaarApi.DTOs;
using BookBazaarApi.Helpers;
using BookBazaarApi.Models;
using BookBazaarApi.Services.Interfaces;
using BookBazaarApi.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BookBazaarApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPost("GetCart")]
        public async Task<IActionResult> GetCart(RequestModel data)
        {
            var cart = await _cartService.GetCartAsync(data.Key);
            var response = new ResponseModel<CartViewModel>
            {
                Success = true,
                Result = cart
            };
            return Ok(response);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] CartAddRequest request)
        {
            await _cartService.AddToCartAsync(request.UserName, request.BookId, request.Quantity);
            var response = new ResponseModel<Cart>
            {
                Success = true,
                Message="Added to cart."
            };
            return Ok(response);
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateCartItem([FromBody] CartUpdateRequest request)
        {
            await _cartService.UpdateCartItemAsync(request.UserName, request.CartItemId, request.Quantity);
            return Ok(new ResponseModel<bool> { Success = true, Result = true });
        }

        [HttpPost("remove")]
        public async Task<IActionResult>  RemoveFromCart(CartUpdateRequest request)
        {
            await _cartService.RemoveFromCartAsync(request.UserName, request.CartItemId);
            return Ok(new ResponseModel<bool> { Success = true, Result = true,Message="Removed Item." });
        }

        [HttpPost("clear")]
        public async Task<IActionResult> ClearCart(RequestModel data)
        {
            await _cartService.ClearCartAsync(data.Key);
            return Ok(new ResponseModel<bool>
            {
                Success = true,
                Result = true,
                Message="Cart Cleared."
            });
        }
        [HttpPost("count")]
        public async Task<IActionResult> GetCartItemCount([FromBody] RequestModel data)
        {
            var count = await _cartService.GetCartItemCountAsync(data.Key);

            return Ok(new ResponseModel<int>
            {
                Success = true,
                Result = count
            });
        }
    }
}
