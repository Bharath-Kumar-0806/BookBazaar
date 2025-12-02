using BookBazaarApi.DAL;
using BookBazaarApi.Helpers;
using BookBazaarApi.Models;
using BookBazaarApi.Services.Classes;
using BookBazaarApi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace BookBazaarApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _dal;
        private readonly PasswordHasherService _passwordHasher;

        public UserController(AppDbContext appDbContext,
                                    PasswordHasherService passwordHasher)
        {
            _dal = appDbContext;
            _passwordHasher = passwordHasher;
        }

        [HttpPost("Get-user-profile")]
        public async Task<ActionResult<ResponseModel<UserProfileVM>>> GetUserProfileAsync(RequestModel data)
        {
            if (string.IsNullOrWhiteSpace(data.Key))
                return BadRequest("username required");

            var user = _dal.Users.FirstOrDefault(u => u.Username == data.Key);
            if (user == null)
                return NotFound("User not found");

            var profile = new UserProfileVM
            {
                UserName = user.Username, // or combine FirstName + LastName
                Email = user.Email,
                //RegisteredAt = user.CreatedAt
            };

            var response = new ResponseModel<UserProfileVM>
            {
                Success = true,
                Result = profile
            };

            return Ok(response);
        }


        [HttpPost("UpdateUserProfile")]
        public async Task<ActionResult<ResponseModel<object>>> UpdateUserProfile(UpdateUserProfileVM data)
        {
            if (string.IsNullOrWhiteSpace(data.OldUserName))
                return BadRequest("Username is required.");

            var user = _dal.Users.FirstOrDefault(u => u.Username == data.OldUserName);
            if (user == null)
                return NotFound("User not found.");


            if (data.NewUserName != null)
            {
                // Check if new username is used by someone else
                if (!string.Equals(data.NewUserName, user.Username, StringComparison.OrdinalIgnoreCase))
                {
                    if (_dal.Users.Any(u => u.Username == data.NewUserName))
                        return Conflict("Username already exists.");
                    user.Username = data.NewUserName;
                }
            }
            if (data.Email != null)
            {
                // Check if new email is used by someone else
                if (!string.Equals(data.Email, user.Email, StringComparison.OrdinalIgnoreCase))
                {
                    if (_dal.Users.Any(u => u.Email == data.Email))
                        return Conflict("Email already registered.");
                    user.Email = data.Email;
                }
            }



            user.UpdatedAt = DateTime.Now;

            // EF tracks changes on `user`, just save
            await _dal.SaveChangesAsync();

            var response = new ResponseModel<object>
            {
                Success = true,
                Message = "User details updated successfully."
            };

            return Ok(response);
        }




        [HttpPost("UpdateUserPassword")]
        public async Task<ActionResult<ResponseModel<object>>> UpdateUserPassword(UpdateUserPasswordVM data)
        {
            if (string.IsNullOrWhiteSpace(data.UserName))
            {
                return BadRequest(new ResponseModel<object>
                {
                    Success = false,
                    Message = "Username is required."
                });
            }

            var user = await _dal.Users.FirstOrDefaultAsync(u => u.Username == data.UserName);
            if (user == null)
            {
                return NotFound(new ResponseModel<object>
                {
                    Success = false,
                    Message = "User not found."
                });
            }

            // Validate current password and update new password
            if (!string.IsNullOrWhiteSpace(data.NewPassword))
            {
                if (!_passwordHasher.VerifyPassword(user, data.CurrentPassword))
                {
                    return Unauthorized(new ResponseModel<object>
                    {
                        Success = false,
                        Message = "Invalid username or password."
                    });
                }

                user.PasswordHash = _passwordHasher.HashPassword(user, data.NewPassword);
            }
            else
            {
                return BadRequest(new ResponseModel<object>
                {
                    Success = false,
                    Message = "New password cannot be empty."
                });
            }

            user.UpdatedAt = DateTime.UtcNow;

            await _dal.SaveChangesAsync();

            return Ok(new ResponseModel<object>
            {
                Success = true,
                Message = "Password changed successfully."
            });
        }


        [HttpPost("AddAddress")]
        public async Task<ActionResult<ResponseModel<object>>> AddAddress(AddressViewModel data)
        {

            var user = await _dal.Users.FirstOrDefaultAsync(u => u.Username == data.UserName);
            if (user == null)
            {
                return NotFound(new ResponseModel<object>
                {
                    Success = false,
                    Message = "User not found."
                });
            }
            var saveAddress = new Address
            {
                UserId = user.Id,
                Full_Name = data.Full_Name,
                Country = data.Country,
                State = data.State,
                City = data.City,
                Street = data.Street,
                House_No = data.House_No,
                Phone = data.Phone,
                Saved_Address = data.SaveNewAddress

            };
            _dal.Addresses.Add(saveAddress);

            await _dal.SaveChangesAsync();

            return Ok(new ResponseModel<object>
            {
                Success = true,
                Result = saveAddress.Id,
                Message = "Address Saved successfully."
            });

        }

        [HttpGet("GetWishList")]
        public async Task<ActionResult> GetWishList(RequestModel model)
        {
            var user = await _dal.Users.FirstOrDefaultAsync(u => u.Username == model.Key);
            if (user == null)
            {
                return NotFound(new ResponseModel<object>
                {
                    Success = false,
                    Message = "User not found."
                });
            }
            var result = _dal.FavoriteBooks
                         .Where(f => f.UserId == user.Id)
                         .Select(f => f.BookId)
                         .ToList();

            var response = new ResponseModel<List<int>>
            {
                Success = true,
                Result = result,
                Message = "Categories retrieved successfully"
            };

            return Ok(response);
        }

        [HttpPost("GetWishListCount")]
        public async Task<ActionResult> GetWishListCount(RequestModel model)
        {
            var user = await _dal.Users.FirstOrDefaultAsync(u => u.Username == model.Key);
            if (user == null)
            {
                return NotFound(new ResponseModel<object>
                {
                    Success = false,
                    Message = "User not found."
                });
            }
            int count = _dal.FavoriteBooks
                .Count(f => f.UserId == user.Id);

            var response = new ResponseModel<int>
            {
                Success = true,
                Result = count
            };

            return Ok(response);
        }

        [HttpPost("AddToWishList")]
        public async Task<ActionResult> AddToWishList(RequestModel model)
        {
            try
            {
                var user = await _dal.Users.FirstOrDefaultAsync(u => u.Username == model.Key);
                if (user == null)
                {
                    return Ok(new ResponseModel<object>
                    {
                        Success = false,
                        Message = "User not found."
                    });
                }
                var book = await _dal.Books.FindAsync(model.Id);
                if (book == null)
                {
                    return Ok(new ResponseModel<object>
                    {
                        Success = false,
                        Message = "Book not found."
                    });
                }
                var wishlist = await _dal.FavoriteBooks.FirstOrDefaultAsync(f => f.UserId == user.Id && f.BookId == model.Id);
                if (wishlist != null)
                {
                    return Ok(new ResponseModel<object>
                    {
                        Success = false,
                        Message = "Already added to wish list."
                    });
                }
                var wishList = new FavoriteBook
                {
                    UserId = user.Id,
                    BookId = book.Id
                };
                _dal.FavoriteBooks.Add(wishList);
                await _dal.SaveChangesAsync();
                return Ok(new ResponseModel<object>
                {
                    Success = true,
                    Result = wishList.Id,
                    Message = "Book added to wish list."
                });
            }
            catch (Exception ex)
            {
                return Ok(new ResponseModel<object>
                {
                    Success = true,
                    Message = ex.Message
                });
            }
        }
        [HttpPost("GetWishListBooks")]
        public async Task<ActionResult> GetWishListBooks(RequestModel model)
        {
            var user = await _dal.Users.FirstOrDefaultAsync(u => u.Username == model.Key);
            if (user == null)
            {
                return Ok(new ResponseModel<List<BookVM>>
                {
                    Success = false,
                    Message = "User not found."
                });
            }
            var favoriteBookIds = await _dal.FavoriteBooks
                    .Where(f => f.UserId == user.Id)
                    .Select(f => f.BookId)
                    .ToListAsync();

            var books = await _dal.Books
             .Where(b => favoriteBookIds.Contains(b.Id))
             .ToListAsync();


            var data = books.Select(b => new BookVM
            {
                Id = b.Id,
                Title = b.Title ?? string.Empty,
                Author = b.Author ?? string.Empty,
                Price = b.Price,
                Stock = b.Stock,
                PhotoPath = b.PhotoPath ?? string.Empty
            }).ToList();

            return Ok(new ResponseModel<List<BookVM>>
            {
                Success = true,
                Message = "Wishlist books retrieved successfully.",
                Result = data
            });

        }
        [HttpPost("RemoveFromWishList")]
        public async Task<ActionResult> RemoveFromWishList(RequestModel model)
        {
            try
            {
                var user = await _dal.Users.FirstOrDefaultAsync(u => u.Username == model.Key);
                if (user == null)
                {
                    return Ok(new ResponseModel<object>
                    {
                        Success = false,
                        Message = "User not found."
                    });
                }
                var book = await _dal.Books.FindAsync(model.Id);
                if (book == null)
                {
                    return Ok(new ResponseModel<object>
                    {
                        Success = false,
                        Message = "Book not found."
                    });
                }
                var favorite = await _dal.FavoriteBooks.FirstOrDefaultAsync(f => f.UserId == user.Id && f.BookId == model.Id);
                if (favorite == null)
                {
                    return Ok(new ResponseModel<object>
                    {
                        Success = false,
                        Message = "Book is not in the wish list."
                    });
                }
                _dal.FavoriteBooks.Remove(favorite);
                await _dal.SaveChangesAsync();
                return Ok(new ResponseModel<object>
                {
                    Success = true,
                    Message = "Book removed from wish list."
                });
            }
            catch (Exception ex)
            {
                return Ok(new ResponseModel<object>
                {
                    Success = true,
                    Message = ex.Message
                });
            }
        }
    }
}