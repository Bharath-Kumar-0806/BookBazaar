using BookBazaarApi.DAL;
using BookBazaarApi.DTOs;
using BookBazaarApi.Helpers;
using BookBazaarApi.Models;
using BookBazaarApi.Services.Classes;
using BookBazaarApi.Services.Interfaces;
using BookBazaarApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookBazaarApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _dal;
        private readonly ICartService _cartService;

        public OrderController(AppDbContext appDbContext, ICartService cartService)
        {
            _dal = appDbContext;
            _cartService = cartService;
        }

        [HttpPost("PlaceOrder")]
        public async Task<IActionResult> PlaceOrder(OrderRequestDto orderDto)
        {
            // Validation
            if (orderDto == null || orderDto.Items == null || !orderDto.Items.Any())
            {
                return BadRequest("Invalid order data.");
            }
            // Check stock availability 
            foreach (var item in orderDto.Items)
            {
                var book = await _dal.Books.FirstOrDefaultAsync(b => b.Id == item.BookId);
                if (book == null || book.Stock < item.Quantity)
                {
                    return BadRequest($"Book '{book?.Title ?? "Unknown"}' does not have enough stock.");
                }
            }
            var user = await _dal.Users.FirstOrDefaultAsync(u => u.Username == orderDto.UserId);

            var deliveryStatusId = await _dal.DeliveryStatuses.FirstOrDefaultAsync(d =>d.Name  == "Pending");

            // Create the order entity
            var order = new Order
            {
                UserId = user.Id,
                OrderDate = DateTime.Now,
                TotalAmount = orderDto.TotalAmount,
                DeliveryStatusId = deliveryStatusId.Id,
                PaymentTypeId =orderDto.PaymentTypeId,
                AddressId = orderDto.AddressId,
                OrderItems = orderDto.Items.Select(item => new OrderItem
                {
                    BookId = item.BookId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                }).ToList()
            };

            // Save to database
            _dal.Orders.Add(order);

            // Update book stock 
            foreach (var item in orderDto.Items)
            {
                var book = await _dal.Books.FirstOrDefaultAsync(b => b.Id == item.BookId);
                book.Stock -= item.Quantity;
            }

            // Save all changes in one go
            await _dal.SaveChangesAsync();


            await _cartService.ClearCartAsync(user.Username);

            var response = new ResponseModel<object>
            {
                Success = true,
                Result = order.Id,
                Message = "Order Placed Successfully"
            };

            return Ok(response);
        }

        [HttpPost("GetMyOrders")]
        public async Task<IActionResult> GetMyOrders(RequestModel data)
        {
            var user = await _dal.Users.FirstOrDefaultAsync(u => u.Username == data.Key);

            if (user == null)
                return Unauthorized(new ResponseModel<List<OrderSummaryVM>>
                {
                    Success = false,
                    Message = "User not found."
                });

            var orders = await _dal.Orders
                .Where(o => o.UserId == user.Id)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Book)
                .Include(o => o.Address)
                .Include(o => o.DeliveryStatus)
                .Include(o => o.PaymentType)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            var deliveryStatusId = await _dal.DeliveryStatuses.FirstOrDefaultAsync(d => d.Name == "Pending");

            var model = orders.Select(o => new OrderSummaryVM
            {
                OrderId = o.Id,
                OrderDate = o.OrderDate,
                DeliveryStatusId =o.DeliveryStatus.Id ,
                DeliveryStatusName = o.DeliveryStatus.Name,
                PaymentTypeId = o.PaymentType.Id,
                PaymentMethodName =o.PaymentType.Name ,
                ShippingAddress = $"{o.Address.House_No}, {o.Address.Street}, {o.Address.City}, {o.Address.State}, {o.Address.Country}",
                Subtotal = o.OrderItems.Sum(i => i.Quantity * i.UnitPrice),
                Items = o.OrderItems.Select(i => new OrderItemVM
                {
                    PhotoPath=i.Book.PhotoPath,
                    BookTitle = i.Book.Title,
                    Quantity = i.Quantity,
                    Price = i.UnitPrice
                }).ToList()
            }).ToList();

            return Ok(new ResponseModel<List<OrderSummaryVM>>
            {
                Success = true,
                Message = "Orders fetched successfully.",
                Result = model
            });
        }
        [HttpPost("GetFilterdOrders")]
        public async Task<IActionResult> GetFilterdOrders(RequestModel data)
        {
            var user = await _dal.Users.FirstOrDefaultAsync(u => u.Username == data.Key);

            if (user == null)
                return Unauthorized(new ResponseModel<List<OrderSummaryVM>>
                {
                    Success = false,
                    Message = "User not found."
                });

            var selectedStatus = _dal.DeliveryStatuses.FirstOrDefault(o => o.Id == data.Id);

            var orders = await _dal.Orders
                .Where(o => o.UserId == user.Id && o.DeliveryStatusId ==data.Id)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Book)
                .Include(o => o.Address)
                .Include(o => o.DeliveryStatus)
                .Include(o => o.PaymentType)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            var deliveryStatusId = await _dal.DeliveryStatuses.FirstOrDefaultAsync(d => d.Name == "Pending");

            var model = orders.Select(o => new OrderSummaryVM
            {
                OrderId = o.Id,
                OrderDate = o.OrderDate,
                DeliveryStatusId = o.DeliveryStatus.Id,
                DeliveryStatusName = o.DeliveryStatus.Name,
                PaymentTypeId = o.PaymentType.Id,
                PaymentMethodName = o.PaymentType.Name,
                ShippingAddress = $"{o.Address.House_No}, {o.Address.Street}, {o.Address.City}, {o.Address.State}, {o.Address.Country}",
                Subtotal = o.OrderItems.Sum(i => i.Quantity * i.UnitPrice),
                Items = o.OrderItems.Select(i => new OrderItemVM
                {
                    PhotoPath = i.Book.PhotoPath,
                    BookTitle = i.Book.Title,
                    Quantity = i.Quantity,
                    Price = i.UnitPrice
                }).ToList()
            }).ToList();

            return Ok(new ResponseModel<List<OrderSummaryVM>>
            {
                Success = true,
                Message = selectedStatus.Name,
                Result = model
            });
        }

        [HttpPost("BuyNow")]
        public async Task<IActionResult> BuyNow(RequestModel data)
        {

            var user = await _dal.Users.FirstOrDefaultAsync(u => u.Username == data.Key);

            var book = await _dal.Books.FirstOrDefaultAsync(b => b.Id == data.Id);
            if (book == null || book.Stock < 1)
            {
                return BadRequest($"Book '{book?.Title ?? "Unknown"}' does not have enough stock.");
            }

            var savedAddresses = await _dal.Addresses
                                            .Where(a => a.Saved_Address == true && a.UserId == user.Id)
                                            .OrderByDescending(a => a.Id)
                                            .ToListAsync();

            var addressViewModels = savedAddresses.Select(a => new CheckOutAddressViewModel
            {
                Id = a.Id,
                FullName = a.Full_Name,
                Country = a.Country,
                State = a.State,
                City = a.City,
                Street = a.Street,
                HouseNo = a.House_No,
                Phone = a.Phone
            }).ToList();

            var defaultAddressId = savedAddresses.FirstOrDefault()?.Id;

            var paymentTypes = await _dal.PaymentTypes.ToListAsync();
            var viewModel = new BuyNowViewModel
            {
                BookId= book.Id,
                BookTitle = book.Title,
                Quantity = 1,
                UnitPrice = book.Price,
                AddressId = defaultAddressId,
                PaymentType = paymentTypes,
                SavedAddresses = addressViewModels
            };


            var response = new ResponseModel<BuyNowViewModel>
            {
                Success = true,
                Result = viewModel,
                Message = "Book Fetched"
            };

            return Ok(response);
        }

        [HttpPost("ConfirmBuyNow")]
        public async Task<IActionResult> ConfirmBuyNow(BuyNowDTO orderDto)
        {
            // Validation
            if (orderDto == null)
            {
                return BadRequest("Invalid order data.");
            }

            // Check if the book exists and has enough stock
            var book = await _dal.Books.FirstOrDefaultAsync(b => b.Id == orderDto.BookId);
            if (book == null)
            {
                return NotFound($"Book with ID {orderDto.BookId} not found.");
            }

            if (book.Stock < orderDto.Quantity)
            {
                return BadRequest($"Not enough stock for '{book.Title}'. Available: {book.Stock}, Requested: {orderDto.Quantity}");
            }

            // Get the user
            var user = await _dal.Users.FirstOrDefaultAsync(u => u.Username == orderDto.UserId);
            if (user == null)
            {
                return NotFound($"User '{orderDto.UserId}' not found.");
            }
            var deliveryStatusId = await _dal.DeliveryStatuses.FirstOrDefaultAsync(d => d.Name == "Pending");
            // Create the order
            var order = new Order
            {
                UserId = user.Id,
                OrderDate = DateTime.Now,
                TotalAmount = orderDto.TotalAmount,
                DeliveryStatusId = deliveryStatusId.Id,
                PaymentTypeId = orderDto.PaymentTypeId,
                AddressId = orderDto.AddressId,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem
                    {
                        BookId = orderDto.BookId,
                        Quantity = orderDto.Quantity,
                        UnitPrice = orderDto.UnitPrice
                    }
                }
            };

            // Save order
            _dal.Orders.Add(order);

            // Update stock
            book.Stock -= orderDto.Quantity;

            // Save changes
            await _dal.SaveChangesAsync();

            var response = new ResponseModel<object>
            {
                Success = true,
                Result = order.Id,
                Message = "Order Placed Successfully"
            };

            return Ok(response);
        }
    }
}
