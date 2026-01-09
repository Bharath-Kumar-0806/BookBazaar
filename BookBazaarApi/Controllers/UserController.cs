using BookBazaarApi.Helpers;
using BookBazaarApi.Services.Interfaces;
using BookBazaarApi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookBazaarApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("Get-user-profile")]
        public async Task<ActionResult<ResponseModel<UserProfileVM>>> GetUserProfileAsync(RequestModel data)
        {
            if (string.IsNullOrWhiteSpace(data.Key))
                return BadRequest("username required");

            var profile = await _userService.GetUserProfileAsync(data.Key);
            if (profile == null)
                return NotFound("User not found");

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
            try
            {
                var result = await _userService.UpdateUserProfileAsync(data);
                if (!result)
                    return NotFound("User not found.");

                return Ok(new ResponseModel<object>
                {
                    Success = true,
                    Message = "User details updated successfully."
                });
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPost("UpdateUserPassword")]
        public async Task<ActionResult<ResponseModel<object>>> UpdateUserPassword(UpdateUserPasswordVM data)
        {
            try
            {
                var result = await _userService.UpdateUserPasswordAsync(data);
                if (!result)
                    return NotFound("User not found.");

                return Ok(new ResponseModel<object>
                {
                    Success = true,
                    Message = "Password changed successfully."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<object> { Success = false, Message = ex.Message });
            }
        }

        [HttpPost("AddAddress")]
        public async Task<ActionResult<ResponseModel<object>>> AddAddress(AddressViewModel data)
        {
            var result = await _userService.AddAddressAsync(data);
            if (result == 0)
                return NotFound("User not found.");

            return Ok(new ResponseModel<object>
            {
                Success = true,
                Result = result,
                Message = "Address Saved successfully."
            });
        }

        [HttpGet("GetWishList")]
        public async Task<ActionResult> GetWishList(RequestModel model)
        {
            var result = await _userService.GetWishListAsync(model.Key);
            if (result == null)
                return NotFound("User not found.");

            return Ok(new ResponseModel<List<int>>
            {
                Success = true,
                Result = result,
                Message = "Wishlist retrieved successfully"
            });
        }

        [HttpPost("GetWishListCount")]
        public async Task<ActionResult> GetWishListCount(RequestModel model)
        {
            var result = await _userService.GetWishListCountAsync(model.Key);
            if (result == -1)
                return NotFound("User not found.");

            return Ok(new ResponseModel<int>
            {
                Success = true,
                Result = result
            });
        }

        [HttpPost("AddToWishList")]
        public async Task<ActionResult> AddToWishList(RequestModel model)
        {
            try
            {
                var result = await _userService.AddToWishListAsync(model.Key, model.Id);
                return Ok(new ResponseModel<object>
                {
                    Success = true,
                    Result = result,
                    Message = "Book added to wish list."
                });
            }
            catch (Exception ex)
            {
                return Ok(new ResponseModel<object> { Success = false, Message = ex.Message });
            }
        }

        [HttpPost("GetWishListBooks")]
        public async Task<ActionResult> GetWishListBooks(RequestModel model)
        {
            var result = await _userService.GetWishListBooksAsync(model.Key);
            if (result == null)
                return NotFound("User not found.");

            return Ok(new ResponseModel<List<BookVM>>
            {
                Success = true,
                Message = "Wishlist books retrieved successfully.",
                Result = result
            });
        }

        [HttpPost("RemoveFromWishList")]
        public async Task<ActionResult> RemoveFromWishList(RequestModel model)
        {
            try
            {
                var result = await _userService.RemoveFromWishListAsync(model.Key, model.Id);
                if (!result)
                    return NotFound("Book not found in wishlist.");

                return Ok(new ResponseModel<object>
                {
                    Success = true,
                    Message = "Book removed from wish list."
                });
            }
            catch (Exception ex)
            {
                return Ok(new ResponseModel<object> { Success = false, Message = ex.Message });
            }
        }
    }
}