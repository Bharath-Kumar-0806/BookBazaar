using BookBazaarApi.Helpers;
using BookBazaarApi.Repos.Interfaces;
using BookBazaarApi.Services.Interfaces;
using BookBazaarApi.ViewModels;
using System.Threading.Tasks;

namespace BookBazaarApi.Services.Classes
{
    public class AdminServices : IAdminServices
    {
        private readonly IAdminRepo _repo;
        public AdminServices(IAdminRepo adminRepo)
        {
            _repo = adminRepo;
        }


        public async Task<AdminDashboardVM> GetDashBoard()
        {
            return await _repo.GetDashBoard();
        }

        public async Task<List<OrdersManageVM>> GetOrders(RequestModel model)
        {
            return await _repo.GetOrders(model);
        }

        public async Task<List<UserViewModel>> GetUsers()
        {
            return await  _repo.GetUsers();
        }

        public  bool UpdateOrderStatus(RequestModel model)
        {
            return  _repo.UpdateOrderStatus(model);
        }
        public async Task<object> DeleteUser(RequestModel data)
        {
            return await _repo.DeleteUser(data);
        }
    }
}
