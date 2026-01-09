using BookBazaarApi.DAL;
using BookBazaarApi.DTOs;
using BookBazaarApi.Models;
using BookBazaarApi.Repos.Interfaces;
using BookBazaarApi.Services.Interfaces;
using BookBazaarApi.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookBazaarApi.Repos.Classes
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;
        private readonly IBookRepository _bookRepository;
        private readonly ICartService _cartService;

        public OrderRepository(AppDbContext context, IBookRepository bookRepository, ICartService cartService)
        {
            _context = context;
            _bookRepository = bookRepository;
            _cartService = cartService;
        }

        public async Task<int> PlaceOrderAsync(OrderRequestDto orderDto)
        {
            foreach (var item in orderDto.Items)
            {
                var book = await _bookRepository.GetByIdAsync(item.BookId);
                if (book == null || book.Stock < item.Quantity)
                {
                    throw new System.Exception($"Book '{book?.Title ?? "Unknown"}' does not have enough stock.");
                }
            }
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == orderDto.UserId);
            var deliveryStatusId = await _context.DeliveryStatuses.FirstOrDefaultAsync(d => d.Name == "Pending");

            var order = new Order
            {
                UserId = user.Id,
                OrderDate = System.DateTime.Now,
                TotalAmount = orderDto.TotalAmount,
                DeliveryStatusId = deliveryStatusId.Id,
                PaymentTypeId = orderDto.PaymentTypeId,
                AddressId = orderDto.AddressId,
                OrderItems = orderDto.Items.Select(item => new OrderItem
                {
                    BookId = item.BookId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                }).ToList()
            };

            await _context.Orders.AddAsync(order);

            foreach (var item in orderDto.Items)
            {
                var book = await _bookRepository.GetByIdAsync(item.BookId);
                book.Stock -= item.Quantity;
                await _bookRepository.UpdateAsync(book);
            }

            await _cartService.ClearCartAsync(user.Username);
            await _context.SaveChangesAsync();
            return order.Id;
        }

        public async Task<List<OrderSummaryVM>> GetMyOrdersAsync(string userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == userId);
            if (user == null)
                return null;

            return await _context.Orders
                .Where(o => o.UserId == user.Id)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Book)
                .Include(o => o.Address)
                .Include(o => o.DeliveryStatus)
                .Include(o => o.PaymentType)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderSummaryVM
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
                }).ToListAsync();
        }

        public async Task<List<OrderSummaryVM>> GetFilteredOrdersAsync(string userId, int statusId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == userId);
            if (user == null)
                return null;

            return await _context.Orders
                .Where(o => o.UserId == user.Id && o.DeliveryStatusId == statusId)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Book)
                .Include(o => o.Address)
                .Include(o => o.DeliveryStatus)
                .Include(o => o.PaymentType)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderSummaryVM
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
                }).ToListAsync();
        }

        public async Task<BuyNowViewModel> GetBuyNowViewModel(string userId, int bookId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == userId);
            var book = await _bookRepository.GetByIdAsync(bookId);

            if (book == null || book.Stock < 1)
            {
                throw new System.Exception($"Book '{book?.Title ?? "Unknown"}' does not have enough stock.");
            }

            var savedAddresses = await _context.Addresses
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

            var paymentTypes = await _context.PaymentTypes.ToListAsync();
            return new BuyNowViewModel
            {
                BookId = book.Id,
                BookTitle = book.Title,
                Quantity = 1,
                UnitPrice = book.Price,
                AddressId = defaultAddressId,
                PaymentType = paymentTypes,
                SavedAddresses = addressViewModels
            };
        }

        public async Task<int> ConfirmBuyNowAsync(BuyNowDTO orderDto)
        {
            var book = await _bookRepository.GetByIdAsync(orderDto.BookId);
            if (book == null)
            {
                throw new System.Exception($"Book with ID {orderDto.BookId} not found.");
            }

            if (book.Stock < orderDto.Quantity)
            {
                throw new System.Exception($"Not enough stock for '{book.Title}'. Available: {book.Stock}, Requested: {orderDto.Quantity}");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == orderDto.UserId);
            if (user == null)
            {
                throw new System.Exception($"User '{orderDto.UserId}' not found.");
            }
            var deliveryStatusId = await _context.DeliveryStatuses.FirstOrDefaultAsync(d => d.Name == "Pending");

            var order = new Order
            {
                UserId = user.Id,
                OrderDate = System.DateTime.Now,
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

            await _context.Orders.AddAsync(order);
            book.Stock -= orderDto.Quantity;
            await _bookRepository.UpdateAsync(book);
            await _context.SaveChangesAsync();
            return order.Id;
        }

        public async Task<bool> UpdateOrderAsync(Order order)
        {
            var existingOrder = await _context.Orders.FindAsync(order.Id);
            if (existingOrder == null)
                return false;

            existingOrder.DeliveryStatusId = order.DeliveryStatusId;

            _context.Orders.Update(existingOrder);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return false;

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _context.Orders.ToListAsync();
        }

        public async Task<Order> GetByIdAsync(int id)
        {
            return await _context.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.Book).FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }
    }
}
