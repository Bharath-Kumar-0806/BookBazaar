using BookBazaarApi.DAL;
using BookBazaarApi.Helpers;
using BookBazaarApi.Models;
using BookBazaarApi.Services.Interfaces;
using BookBazaarApi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Tracing;


namespace BookBazaarApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminServices _adminServices;
        private readonly AppDbContext _dal;
        public AdminController(IAdminServices adminServices,
                               AppDbContext dal)
        {
            _adminServices = adminServices;
            _dal = dal;
        }
        [HttpGet("Dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var result = await _adminServices.GetDashBoard();
            if (result != null)
            {
                var successResponse = new ResponseModel<AdminDashboardVM>
                {
                    Success = true,
                    Result = result
                };
                return Ok(successResponse);
            }
            var errorResponse = new ResponseModel<AdminDashboardVM>
            {
                Success = false,
                Result = null,
                Message="Unable to load dashboard."

            };
            return Ok(errorResponse);

        }

        [HttpGet("UserManagement")]
        public async Task<IActionResult> UserManagement()
        {
            var result = await _adminServices.GetUsers();
            if (result != null)
            {
                var successResponse = new ResponseModel<List<UserViewModel>>
                {
                    Success = true,
                    Result = result
                };
                return Ok(successResponse);
            }
            var errorResponse = new ResponseModel<List<UserViewModel>>
            {
                Success = false,
                Result = null,
                Message = "Unable to load dashboard."

            };
            return Ok(errorResponse);
        }

        [HttpPost("DeleteUser")]
        public async Task<ActionResult<ResponseModel<object>>> DeleteUser(RequestModel data)
        {
            dynamic result = await _adminServices.DeleteUser(data);
            if (result.Success)
            {
                var successResponse = new ResponseModel<object>
                {
                    Success = true,
                    Message = "User Deleted Successfully."
                };
                return Ok(successResponse);
            }
            var errorResponse = new ResponseModel<object>
            {
                Success = false,
                Message = result.Message

            };
            return Ok(errorResponse);

        }

        [HttpPost("GetOrders")]
        public async Task<ActionResult> GetOrders(RequestModel model)
        {
            var result = await _adminServices.GetOrders(model);

            var selectedStatus = _dal.DeliveryStatuses.FirstOrDefault(o => o.Id == model.Id);
            selectedStatus ??= new DeliveryStatus { Name = "All" };

            if (result != null)
            {
                var successResponse = new ResponseModel<List<OrdersManageVM>>
                {
                    Success = true,
                    Result = result,
                    Message = selectedStatus.Name
                };
                return Ok(successResponse);
            }
            var errorResponse = new ResponseModel<List<OrdersManageVM>>
            {
                Success = false,
                Result = null,
                Message = "Unable to load dashboard."

            };
            return Ok(errorResponse);
        }

        [HttpPost("UpdateOrderStatus")]
        public async Task<ActionResult> UpdateOrderStatus(RequestModel model)
        {
            var result =  _adminServices.UpdateOrderStatus(model);

            var order = _dal.Orders.FirstOrDefault(o => o.Id == model.Id);
            if (result)
            {
                var successResponse = new ResponseModel<object>
                {
                    Success = true,
                    Message = "Status updated Successfully."
                };
                return Ok(successResponse);
            }
            var errorResponse = new ResponseModel<object>
            {
                Success = false,
                Message = "Invalid Order details."
            };
            return Ok(errorResponse);
        }
    }
}
