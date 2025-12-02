using BookBazaar.DTOs;
using BookBazaar.Helpers;
using BookBazaar.Models;
using BookBazaar.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace BookBazaar.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ApiHelper _apiHelper;
        public OrdersController(IConfiguration configuration)
        {
            _apiHelper = new ApiHelper(configuration);
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {
            var userId = User.Identity.Name;

            int shippingAddressId ;

            // User selected address from DB
            if (model.SelectedAddressId.HasValue)
            {
                shippingAddressId = model.SelectedAddressId.Value;
            }
            // User entered a new address
            else if (!string.IsNullOrWhiteSpace(model.Full_Name))
            {
                var addressData = new Address
                {
                    UserName = userId,
                    Full_Name = model.Full_Name,
                    Country = model.Country,
                    State = model.State,
                    City = model.City,
                    Street = model.Street,
                    House_No = model.House_No,
                    Phone = model.Phone,
                    SaveNewAddress = model.SaveNewAddress
                };

                var addressResponse = await _apiHelper.ApiCall<object>("User/AddAddress", addressData);
                if (!addressResponse.Success)
                {
                    TempData["error"] = addressResponse.Message;
                    return RedirectToAction("CheckOut", "Cart");
                }
                TempData["success"] = addressResponse.Message;
                shippingAddressId = Convert.ToInt32(addressResponse.Result);
            }
            //No address provided
            else
            {
                TempData["ErrorMessage"] = "Please select or enter a shipping address.";
                return RedirectToAction("Checkout","Cart");
            }

            var data = new RequestModel
            {
                Key = userId
            };
            // Get cart items
            var cartItems = await _apiHelper.ApiCall<CartViewModel>("cart/GetCart", data);

            if (cartItems == null || !cartItems.Result.Items.Any())
                return RedirectToAction("Index", "Cart");

            // Create the order request
            var orderRequest = new OrderRequestDto
            {
                UserId = userId,
                TotalAmount = cartItems.Result.Total,
                PaymentTypeId = model.PaymentTypeId,
                AddressId= shippingAddressId,
                Items = cartItems.Result.Items.Select(i => new OrderItemDto
                {
                    BookId = i.BookId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };
            var response = await _apiHelper.ApiCall<object>("Order/PlaceOrder", orderRequest);
            ViewData["OrderId"] = response.Result;

            return View("Index");
        }

        public async Task<ActionResult> GetMyOrders()
        {
            var request = new RequestModel
            {
                Key = User.Identity.Name
            };

            var response = await _apiHelper.ApiCall<List<OrderSummaryVM>>("Order/GetMyOrders", request);
            if (!response.Success)
            {
                TempData["error"] = "Something went wrong while fetching your orders.";
                return RedirectToAction("Index", "Home");
            }
            return View(response.Result);
        }

        [HttpPost]
        public async Task<ActionResult> GetFilterdOrders(int status)
        {
            if(status == 0)
            {
                return RedirectToAction("GetMyOrders");
            }
            var request = new RequestModel
            {
                Key = User.Identity.Name ,
                Id=status
            };

            var response = await _apiHelper.ApiCall<List<OrderSummaryVM>>("Order/GetFilterdOrders", request);
            if (!response.Success)
            {
                TempData["error"] = "Something went wrong while fetching your orders.";
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Status = response.Message;
            return View("GetMyOrders",response.Result);
        }

        public async Task<ActionResult> BuyNow(int bookId)
        {
            var data = new RequestModel
            {
                Id=bookId,
                Key = User.Identity.Name
            };
            var response = await _apiHelper.ApiCall<BuyNowViewModel>("Order/BuyNow", data);
            return View(response.Result);
        }

        [HttpPost]
        public async Task<ActionResult> ConfirmBuyNow(BuyNowPlaceOrderViewModel model)
        {
            var userId = User.Identity.Name;

            int shippingAddressId;

            // User selected address from DB
            if (model.SelectedAddressId.HasValue)
            {
                shippingAddressId = model.SelectedAddressId.Value;
            }
            // User entered a new address
            else if (!string.IsNullOrWhiteSpace(model.Full_Name))
            {
                var addressData = new Address
                {
                    UserName = userId,
                    Full_Name = model.Full_Name,
                    Country = model.Country,
                    State = model.State,
                    City = model.City,
                    Street = model.Street,
                    House_No = model.House_No,
                    Phone = model.Phone,
                    SaveNewAddress = model.SaveNewAddress
                };

                 var addressResponse = await _apiHelper.ApiCall<object>("User/AddAddress", addressData);
                if (!addressResponse.Success)
                {
                    TempData["ErrorMessage"] = "Unable to save address";
                    return RedirectToAction("BuyNow", new { bookId = model.BookId });
                }
                shippingAddressId = Convert.ToInt32(addressResponse.Result);
            }
            //No address provided
            else
            {
                TempData["ErrorMessage"] = "Please select or enter a shipping address.";
                return RedirectToAction("BuyNow", new { bookId = model.BookId }); 
            }
            var orderRequest = new BuyNowDTO
            {
                UserId = userId,
                TotalAmount = model.Total,
                PaymentTypeId = model.PaymentTypeId,
                AddressId = shippingAddressId,
                BookId = model.BookId,
                Quantity = model.Quantity,
                UnitPrice = model.UnitPrice
            };
            var response = await _apiHelper.ApiCall<object>("Order/ConfirmBuyNow", orderRequest);
            ViewData["OrderId"] = response.Result;
            return View("Index");
        }
    }
}
