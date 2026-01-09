using BookBazaar.Helpers;
using BookBazaar.Models;
using BookBazaar.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BookBazaar.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApiHelper _apiHelper;
        public HomeController(IConfiguration configuration)
        {
            _apiHelper = new ApiHelper(configuration);
        }

        public async Task<IActionResult> GetCartCount()
        {
            var data = new RequestModel
            {
                Key = User.Identity.Name
            };
            var response = await _apiHelper.ApiCall<int>("cart/count", data);
            if(response != null && response.Success)
            {
                return Json(new { success = true, result = response.Result });
            }
            return Json(new { success = false});
        }

        public async Task<IActionResult> GetWishListCount()
        {
            var data = new RequestModel
            {
                Key = User.Identity.Name
            };
            var response = await _apiHelper.ApiCall<int>("User/GetWishListCount", data);
            if (response != null && response.Success)
            {
                return Json(new { success = true, result = response.Result });
            }
            return Json(new { success = false });
        }

        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Admin");
            }
            var request = new RequestModel
            {
                Key = User.Identity.Name 
            };
            var response = await _apiHelper.ApiCall<UserHomeVM>("Books/HomePageBooks", request);

            if (response != null && response.Success)
            {
                return View(response.Result); 
            }
           

            TempData["error"] = "Something went wrong while fetching new arrivals.";
            return View(new List<BookVM>()); 
        }
        
        public IActionResult Privacy()
        {
            return View();
        }

        [Route("/Home/Error")]
        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }

        //loads nav bar categories
        public async Task<IActionResult> GetCategories()
        {
            var response = await _apiHelper.ApiCall<List<Category>>("category/getcategories");
            if (!response.Success)
            {
                ViewBag.Error = response.Message;
                return View(new List<Category>());
            }
            return Json(new { success = true, result = response.Result });
            //return View(response.Result);
        }

        public async Task<IActionResult> GetNewArrivalsViewAll()
        {
            var request = new RequestModel
            {
                Key = User.Identity.Name
            };
            var response = await _apiHelper.ApiCall<List<BookVM>>("Books/GetNewArrivalsViewAll", request);
            if (response != null && response.Success)
            {
                return View("NewArrivalsViewAll", response.Result);
            }
            TempData["error"] = "Something went wrong while fetching new arrivals books";
            return View("Index");
        }

        public async Task<IActionResult> GetBestSellersViewAll()
        {
            var request = new RequestModel
            {
                Key = User.Identity.Name
            };
            var response = await _apiHelper.ApiCall<List<BookVM>>("Books/GetBestSellersViewAll", request);
            if (response != null && response.Success)
            {
                return View("GetBestSellersViewAll", response.Result);
            }
            TempData["error"] = "Something went wrong while fetching best sellers books";
            return View("Index");
        }

        //book details page
        public async Task<IActionResult> BookDetails(int id)
        {
            var data = new RequestModel
            {
                Id = id
            };
            var book = await _apiHelper.ApiCall<BookDetailsVM>("Books/GetBookDetails", data);
            return View(book.Result);
        }

        public async Task<IActionResult> GetBooksByCategoryId(int id)
        {
            var data = new RequestModel
            {
                Id = id,
                Key = User.Identity.Name
            };
            var book = await _apiHelper.ApiCall<List<BookVM>>("Books/GetBooksByCategoryId", data);
            return View("CategoryBooks",book.Result);
        }

        [HttpPost]
        public async Task<ActionResult> AddToWishList(int bookId)
        {
            var data = new RequestModel
            {
                Key = User.Identity.Name,
                 Id = bookId
            };
            var response = await _apiHelper.ApiCall<object>("User/AddToWishList", data);
            if (!response.Success)
            {
                TempData["error"] = response.Message;
                return Json(new { success = true });
            }
            TempData["success"] = response.Message;
            return Json(new { success = true, result = response.Result });
        }

        public async Task<IActionResult> GetWishListBooks()
        {
            var data = new RequestModel
            {
                Key = User.Identity.Name
            };
            var response = await _apiHelper.ApiCall<List<BookVM>>("User/GetWishListBooks", data);
            if (response != null && response.Success)
            {
                return View("WishList", response.Result);
            }
            TempData["error"] = "Something went wrong while fetching wish list";
            return View("Index");
        }

        public async Task<IActionResult> RemoveFromWishList(int bookId)
        {
            var data = new RequestModel
            {
                Key = User.Identity.Name,
                Id=bookId
            };
            var response = await _apiHelper.ApiCall<object>("User/RemoveFromWishList", data);
            if (response != null && response.Success)
            {
                TempData["success"] = response.Message;
                return Json(new { success = true });
            }
            TempData["error"] = "something went wrong while removing from wishlist";
            return Json(new { success = false });
        }

        public async Task<IActionResult> SearchResults(string search)
        {
            if(search != null)
            {
                var data = new RequestModel
                {
                    Key = search
                };
                var response = await _apiHelper.ApiCall<List<BookVM>>("Books/SearchResults", data);
                if (response != null && response.Success)
                {
                    ViewBag.Search = response.Message;
                    return View(response.Result);
                }
                ViewBag.Search = search;
            }
            return RedirectToAction("Index");
        }
    }
}
