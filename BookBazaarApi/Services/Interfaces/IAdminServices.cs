using BookBazaarApi.Helpers;
using BookBazaarApi.ViewModels;

namespace BookBazaarApi.Services.Interfaces
{
    public interface IAdminServices
    {
        Task<AdminDashboardVM> GetDashBoard();

        Task<List<UserViewModel>> GetUsers();

        Task<object> DeleteUser(RequestModel data);

        Task<List<OrdersManageVM>> GetOrders(RequestModel model);

        bool UpdateOrderStatus(RequestModel model);
    }
}
