using BookBazaar.DTOs;
using BookBazaar.Helpers;
using BookBazaar.Models;
using BookBazaar.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookBazaar.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApiHelper _apiHelper;
        public CartController(IConfiguration configuration)
        {
            _apiHelper = new ApiHelper(configuration);
        }
        public async Task<IActionResult> Cart()
        {
            var userName = User.Identity.Name;
            var data = new RequestModel
            {
                Key = userName
            };
            var response = await _apiHelper.ApiCall<CartViewModel>("cart/GetCart", data);
            return View(response.Result);
        }
        [HttpPost]
        public async Task<ActionResult> AddToCart(int bookId)
        {
            var data = new CartAddRequest
            {
                UserName = User.Identity.Name,
                BookId = bookId,
                Quantity = 1
            };

            var response = await _apiHelper.ApiCall<Cart>("cart/add", data);
            if (!response.Success)
            {
                TempData["error"] = "Unable to add to cart.";
                return Json(new { success = false });
            }
            TempData["success"] = response.Message;
            return Json(new { success = true, result = response.Result });
        }


        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int id, int quantity)
        {
            var data = new CartUpdateRequest
            {
                UserName = User.Identity.Name,
                CartItemId = id,
                Quantity = quantity
            };

            var response= await _apiHelper.ApiCall<bool>("cart/update", data);
            if (!response.Success)
            {
                TempData["error"] = "Something Went wrong";
                return RedirectToAction("Cart");
            }
            TempData["success"] = response.Message;
            return RedirectToAction("Cart");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var data = new CartUpdateRequest
            {
                UserName = User.Identity.Name,
                CartItemId = id
            };

            var response=await _apiHelper.ApiCall<bool>("cart/remove", data);
            if (!response.Success)
            {
                TempData["error"] = "Something Went wrong";
                return RedirectToAction("Cart");

            }
            TempData["success"] = response.Message;
            return RedirectToAction("Cart");
        }
        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            var data = new RequestModel
            {
                Key = User.Identity.Name
            };

            var result = await _apiHelper.ApiCall<bool>("cart/clear", data);

            if (!result.Success)
            {
                TempData["error"] = "Something Went wrong while clearing your cart";
                return RedirectToAction("Cart");
            }
            TempData["success"] = result.Message;
            return RedirectToAction("Cart");
        }

        [HttpGet]
        public async Task<IActionResult> CheckOut()
        {

            var data = new RequestModel
            {
                Key = User.Identity.Name
            };
            var response = await _apiHelper.ApiCall<CartViewModel>("cart/GetCart", data);
            return View(response.Result);
        }

        [HttpPost]
        public async Task<IActionResult> BuyNow(int bookId)
        {
            var cartResponse = await AddToCart(bookId) as JsonResult;
            if (cartResponse != null)
            {
                var data = cartResponse.Value as dynamic;

                if (data.success == true)
                {
                    return Json(new { success = true, message = "Proceeding to checkout...", redirectUrl = Url.Action("CheckOut","Cart") });
                }
            }
            return Json(new { success = false, message = "Unable to add to cart." });
        }
    }
}
