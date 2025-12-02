using BookBazaarApi.DAL;
using BookBazaarApi.Helpers;
using BookBazaarApi.Models;
using BookBazaarApi.Repos.Interfaces;
using BookBazaarApi.Services.Interfaces;
using BookBazaarApi.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BookBazaarApi.Repos.Classes
{
    public class AdminRepo : IAdminRepo
    {
        private readonly AppDbContext _dal;
        public AdminRepo(AppDbContext appDbContext)
        {
            _dal = appDbContext;
        }

        public async Task<AdminDashboardVM> GetDashBoard()
        {
            var model = new AdminDashboardVM
            {
                TotalBooks = await _dal.Books.CountAsync(),
                TotalCategories = await _dal.Categories.CountAsync(),
                TotalUsers = await _dal.Users.CountAsync(u => u.Username != "admin"),

                RecentBooks = await _dal.Books
                                  .OrderByDescending(b => b.Id)
                                  .Take(5)
                                  .ToListAsync()
            };
            return model;
        }

        public async Task<List<OrdersManageVM>> GetOrders(RequestModel model)
        {
            var deliveryStatuses = await _dal.DeliveryStatuses.ToListAsync();

            var selectedStatus = _dal.DeliveryStatuses.FirstOrDefault(o => o.Id == model.Id);
            selectedStatus ??= new DeliveryStatus { Name = "All" };
            var query = from o in _dal.Orders
                        join u in _dal.Users on o.UserId equals u.Id
                        join d in _dal.DeliveryStatuses on o.DeliveryStatusId equals d.Id
                        where model.Id == 0 || o.DeliveryStatusId == model.Id
                        select new OrdersManageVM
                        {
                            OrderId = o.Id,
                            CustomerName = u.Username,
                            Email = u.Email,
                            TotalAmount = o.TotalAmount,
                            OrderDate = o.OrderDate,
                            Status = d.Name,
                            DeliveryStatusId = o.DeliveryStatusId,
                            AvailableStatuses = deliveryStatuses
                        };

            var orders = await query.ToListAsync();
            return orders;
        }

        public async Task<List<UserViewModel>> GetUsers()
        {
            var users = await _dal.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .ToListAsync();

            var viewModel = users.Select(u => new UserViewModel
            {
                Id = u.Id,
                UserName = u.Username,
                Email = u.Email,
                Role = u.UserRoles.Select(ur => ur.Role.Name).FirstOrDefault() ?? "User",
                IsActive = u.IsActive
            }).ToList();
            return viewModel;
        }

        public bool UpdateOrderStatus(RequestModel model)
        {
            var order = _dal.Orders.FirstOrDefault(o => o.Id == model.Id);
            if (order != null)
            {
                order.DeliveryStatusId = Convert.ToInt32(model.Key);
                _dal.SaveChanges();
                return true;
            }
            return false;
        }
        public async Task<object> DeleteUser(RequestModel data)
        {
            try
            {
                var user = await _dal.Users.FirstOrDefaultAsync(u => u.Id == data.Id);
                if (user == null)
                {
                    return new
                    {
                        Success = false,
                        Message = "User not found."
                    };
                }

                _dal.Users.Remove(user);
                await _dal.SaveChangesAsync();
                return new
                {
                    Success = true,
                    Message = "User deleted successfully."
                };
            }
            catch(Exception ex) 
            {
                return new
                {
                    Success = false,
                    Message = ex.Message
                };
            }
           
        }
    }
}
