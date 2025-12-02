using BookBazaarApi.DAL;
using BookBazaarApi.Models;
using BookBazaarApi.Services.Interfaces;
using BookBazaarApi.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BookBazaarApi.Services.Classes
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _context;

        public CartService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<CartViewModel> GetCartAsync(string userName)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Book)
                .FirstOrDefaultAsync(c => c.UserName == userName);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == userName);

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

            var items = cart?.CartItems.Select(ci => new CartItemViewModel
            {
                Id = ci.Id,
                BookId = ci.Book.Id,
                BookTitle = ci.Book.Title ?? "Unknown",
                Author = ci.Book.Author ?? "Unknown",
                Quantity = ci.Quantity,
                UnitPrice = ci.UnitPrice,
                ImageUrl = ci.Book.PhotoPath
            }).ToList() ?? new List<CartItemViewModel>();

            var viewModel = new CartViewModel
            {
                Items = items,
                AddressId = defaultAddressId,
                PaymentType= paymentTypes,
                SavedAddresses = addressViewModels
            };


            return viewModel;
        }

        // ✅ Change return type to Task<Cart> to return the updated cart
        public async Task<Cart> AddToCartAsync(string userName, int bookId, int quantity)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserName == userName);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserName = userName,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.BookId == bookId);

            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                var book = await _context.Books.FindAsync(bookId);
                if (book == null)
                    throw new Exception("Book not found");

                cartItem = new CartItem
                {
                    CartId = cart.Id,
                    BookId = bookId,
                    Quantity = quantity,
                    UnitPrice = book.Price
                };
                _context.CartItems.Add(cartItem);
            }

            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Return the updated cart
            return cart;
        }

        // ✅ Return true if removal successful
        public async Task<bool> RemoveFromCartAsync(string userName, int cartItemId)
        {
            var cartItem = await _context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.Cart.UserName == userName);

            if (cartItem == null)
                return false;

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return true;
        }

        // ✅ Return true if update successful
        public async Task<bool> UpdateCartItemAsync(string userName, int cartItemId, int quantity)
        {
            var cartItem = await _context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.Cart.UserName == userName);

            if (cartItem == null)
                return false;

            cartItem.Quantity = quantity;
            await _context.SaveChangesAsync();
            return true;
        }

        // ✅ Return true if cart was cleared
        public async Task<bool> ClearCartAsync(string userName)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserName == userName);

            if (cart != null && cart.CartItems.Any())
            {
                _context.CartItems.RemoveRange(cart.CartItems);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
        public async Task<int> GetCartItemCountAsync(string userName)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserName == userName);

            return cart?.CartItems.Sum(ci => ci.Quantity) ?? 0;
        }
    }
}
