using BookBazaar.Helpers;
using BookBazaar.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace BookBazaar.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApiHelper _apiHelper;
        public AdminController(IConfiguration configuration)
        {
            _apiHelper = new ApiHelper(configuration);
        }
        public async Task<IActionResult> Index()
        {
            var response = await _apiHelper.ApiCall<AdminDashboardVM>("Admin/Dashboard");
            if (response != null && response.Success)
            {
                return View(response.Result);
            }
            return View();
        }
        public IActionResult AddBook()
        {
            return View();
        }
        public async Task<IActionResult> ManageUser()
        {
            var response = await _apiHelper.ApiCall<List<UserViewModel>>("Admin/UserManagement");
            if (response != null && response.Success)
            {
                return View(response.Result);
            }
            return View();
        }

        public async Task<IActionResult> DeleteUser(int id)
        {
            var data = new RequestModel
            {
                Id = id
            };
            var response = await _apiHelper.ApiCall<object>("Admin/DeleteUser", data);
            if (response != null && response.Success)
            {
                TempData["success"] = response.Message;
                return RedirectToAction("ManageUser");
            }
            TempData["error"] = "something went wrong";
            return View("ManageUser");
        }
        public async Task<ActionResult> Settings()
        {
            return View();
        }
        public async Task<IActionResult> UpdateUserProfile(UpdateUserProfileVM obj)
        {
            if (ModelState.IsValid)
            {
                var username = User.Identity.Name;
                var data = new UpdateUserProfileVM
                {
                    OldUserName = username,
                    NewUserName = obj.NewUserName,
                    Email = obj.Email,
                };

                var response = await _apiHelper.ApiCall<object>("User/UpdateUserProfile", data);


                if (response == null || !response.Success)
                {
                    ViewBag.Message = response.Message;
                    return View("Settings");
                }
                return View("Settings");
            }
            return View("Settings");

        }
        public async  Task<ActionResult> Orders(int status)
        {
            var request = new RequestModel
            {
                Id = status
            };
            var response = await _apiHelper.ApiCall<List<OrdersManageVM>>("Admin/GetOrders", request);
            if (response != null && response.Success)
            {
                ViewBag.ErrorMessage = response.Message;
                return View(response.Result);
            }
            return View(new List<OrdersManageVM>());
        }

        public async Task<ActionResult> UpdateOrderStatus(int orderId, string newStatus)
        {
            var request = new RequestModel
            {
                Id = orderId,
                Key = newStatus
            };
            var response = await _apiHelper.ApiCall<object>("Admin/UpdateOrderStatus", request);
            if (response != null && response.Success)
            {
                TempData["success"] = response.Message;
                return RedirectToAction("Orders");
            }
            TempData["error"] = response.Message;
            return RedirectToAction("Orders");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
