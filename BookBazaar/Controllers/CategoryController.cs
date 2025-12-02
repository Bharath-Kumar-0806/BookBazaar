using BookBazaar.Helpers;
using BookBazaar.Models;
using Microsoft.AspNetCore.Mvc;


namespace BookBazaar.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApiHelper _apiHelper;
        public CategoryController(IConfiguration configuration)
        {
            _apiHelper = new ApiHelper(configuration);
        }
        public async Task<IActionResult> Index()
        {
           var response = await _apiHelper.ApiCall<List<Category>>("Category/GetCategories");
            if (response != null && response.Success)
            {
                return View(response.Result); 
            }
            return View(new List<Category>());
        }
        public IActionResult Create()
        {
            return View();
        }
        //save
        [HttpPost]
        public async Task<IActionResult> SaveCategory(Category obj)
        {
            if (ModelState.IsValid)
            {
                var response = await _apiHelper.ApiCall<List<Category>>("Category/SaveCategory",obj);
                if (response.Success)
                {
                    TempData["success"] = "Category created successfully.";
                    return RedirectToAction("Index");
                }
            }
            TempData["error"] = "Unable to create.";
            return View("Create");
        }
        public async Task<IActionResult> Edit(int id)
        {
            var data = new RequestModel
            {
                Id = id
            };
            var category = await _apiHelper.ApiCall<Category>("Category/GetCategoryById", data);
            return View(category.Result);
        }
        [HttpPost]
        public async Task<IActionResult> EditPost(Category data)
        {
            if (ModelState.IsValid)
            {
               var result = await _apiHelper.ApiCall<List<Category>>("Category/EditCategory", data);
                if (result.Success) {
                    TempData["success"] = "Category edited successfully.";
                    return RedirectToAction("Index");
                }
            }
            TempData["error"] = "Edit failed.";
            return View("Edit");

        }


        [HttpPost]
        public async  Task<IActionResult> DeletePost(int id)
        {
            var data = new RequestModel
            {
                Id = id
            };
            var response = await _apiHelper.ApiCall<List<object>>("Category/DeleteCategory", data);
            if (!response.Success)
            {
                TempData["error"] = "Unable to delete.";
            }
            TempData["success"] = "Category deleted successfully.";
            return RedirectToAction("Index");
        }


    }
}
