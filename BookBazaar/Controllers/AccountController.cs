using BookBazaar.DTOs;
using BookBazaar.Helpers;
using BookBazaar.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookBazaar.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApiHelper _apiHelper;
        public AccountController(IConfiguration configuration)
        {
            _apiHelper = new ApiHelper(configuration);
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var response = await _apiHelper.ApiCall<LoginResponseDTO>("auth/login", model);

            if (!response.Success || response.Result == null)
            {
                ModelState.AddModelError("", "Invalid Email or Password.");
                return View(model);
            }

            var userInfo = response.Result;
            // Create claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userInfo.Username),
                new Claim(ClaimTypes.NameIdentifier, userInfo.UserId.ToString())
            };

            // Optional: add role claims
            foreach (var role in userInfo.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                
                IsPersistent = model.RememberMe, 
                ExpiresUtc = model.RememberMe
                    ? DateTime.UtcNow.AddDays(7) 
                    : DateTime.UtcNow.AddHours(1) 
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties
            );

            if (userInfo.Roles.Contains("Admin"))
            {
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                return RedirectToAction("Index", "Home");

            }
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match.");
                return View(model);
            }

            var registerRequest = new RegisterDTO
            {
                Username = model.Username,
                Email = model.Email,
                Password = model.Password
               
            };
            var response = await _apiHelper.ApiCall<RegisterDTO>("auth/register", registerRequest);

            if (!response.Success || response.Result == null)
            {
                //foreach(var message in response.Message.Split(';'))
                //{
                //    if (string.IsNullOrEmpty(message)) continue;

                //    var splitMessage = message.Split(':');
                //    ModelState.AddModelError(splitMessage[0], splitMessage[1] ?? "Registration failed");
                //}
                //ViewBag.ErrorMessage = response.Message;
                //ModelState.AddModelError("General", response.Message ?? "Registration failed");


                var splitMessage = response.Message.Split(':');
                ModelState.AddModelError(splitMessage[0], splitMessage[1] ?? "Registration failed");
            
                return View(model);
            }

            return RedirectToAction("Login", "Account");
        }
   

        [HttpGet]
        public  IActionResult UserSettings()
        {

            return View();
        }

        public async Task<IActionResult> GetUserDetails()
        {
            var data = new RequestModel
            {
                Key = User.Identity.Name
            };
            var response = await _apiHelper.ApiCall<UserProfileVM>("User/get-user-profile", data);
            if (response != null && response.Success)
            {
                return Json(new { success = true, result = response.Result });
            }
            return Json(new { success = false });
        }


        //update user information
        [Authorize]
        [HttpPost]
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
                    return View("UserSettings");
                }
               
                return RedirectToAction("UserSettings");
            }
            return View("UserSettings");

        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateUserPassword(UpdateUserPasswordVM obj)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );

                return Json(new { success = false, errors });
            }
            var data = new UpdateUserPasswordVM
            {
                UserName = User.Identity.Name,
                CurrentPassword = obj.CurrentPassword,
                NewPassword = obj.NewPassword,
                ConfirmNewPassword = obj.ConfirmNewPassword,
            };

            var response = await _apiHelper.ApiCall<object>("User/UpdateUserPassword", data);

            if (response == null || !response.Success)
            {
                return Json(new { success = false, message = "Something went wrong" });
            }

            return Json(new { success = true, message = "Password updated successfully!" });

        }
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
