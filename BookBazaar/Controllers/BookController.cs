using BookBazaar.Helpers;
using BookBazaar.Models;
using BookBazaar.ViewModels;
using Microsoft.AspNetCore.Mvc;


namespace BookBazaar.Controllers
{
    public class BookController : Controller
    {
        private readonly ApiHelper _apiHelper;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public BookController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            _apiHelper = new ApiHelper(configuration);
            _hostingEnvironment = hostingEnvironment;
        }
        public async Task<IActionResult> BooksList()
        {
            var response = await _apiHelper.ApiCall<List<Books>>("Books/GetBooks");
            if (response != null && response.Success)
            {
                return View("Book", response.Result);
            }

            return View("Book");
        }
        public async Task<IActionResult> SearchBook(string search)
        {
            var data = new RequestModel
            {
                Key = search
            };
            var response = await _apiHelper.ApiCall<List<Books>>("Books/SearchBook", data);
            if (response != null && response.Success)
            {
                ViewBag.Search = search;
                return View("Book", response.Result);
            }
            return View("Book");
        }
        public bool CheckAndUploadFile(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                var path = Path.GetDirectoryName(filePath);
                var extension = Path.GetExtension(filePath);
                var filename = Guid.NewGuid().ToString();

                var fullPath = Path.Combine(path,filename + "." + extension);
                CheckAndUploadFile(fullPath);
            }
            return true;
        }
        public IActionResult CreateBook()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SaveBook(CreateBookVM data)
        {
            if (ModelState.IsValid)
            { 
                string uniqueFileName = null;
                if (data.Photo != null)
                {
                    string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + data.Photo.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    data.Photo.CopyTo(new FileStream(filePath, FileMode.Create));
                }

                Books newBook = new()
                {
                    Title = data.Title,
                    Author = data.Author,
                    Description = data.Description,
                    ISBN = data.ISBN,
                    Price = data.Price,
                    Stock = data.Stock,
                    CategoryId = data.CategoryId,
                    PhotoPath = uniqueFileName
                };
                var response = await _apiHelper.ApiCall<ResponseModel<CreateBookVM>>("books/SaveBook", newBook);
                if(response.Success)
                {
                    TempData["success"] = response.Message;
                    return RedirectToAction("BooksList");
                }
            }
            TempData["error"] = "Unable to add book.";
            return View("CreateBook");

        }
        public async Task<IActionResult> Edit(int id)
        {
            var data = new RequestModel
            {
                Id = id
            };
            var book = await _apiHelper.ApiCall<Books>("Books/GetByIdAsync", data);
            return View(book.Result);
        }
        [HttpPost]
        public async Task<IActionResult> EditPost(EditBookVM data)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = data.PhotoPath;
                if (data.Photo != null)
                {
                    string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + data.Photo.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    data.Photo.CopyTo(new FileStream(filePath, FileMode.Create));
                }

                Books newBook = new()
                {
                    Id=data.Id,
                    Title = data.Title,
                    Author = data.Author,
                    Description = data.Description,
                    ISBN = data.ISBN,
                    Price = data.Price,
                    Stock = data.Stock,
                    CategoryId = data.CategoryId,
                    PhotoPath = uniqueFileName  
                };
                var result = await _apiHelper.ApiCall<CreateBookVM>("Books/EditBook", newBook);
                if (result.Success)
                {
                    TempData["success"] = "Book edited successfully.";
                    return RedirectToAction("BooksList");
                }
            }
            TempData["error"] = "Edit failed.";
            return View("Edit");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var data = new RequestModel
            {
                Id = id
            };
            var response = await _apiHelper.ApiCall<List<object>>("Books/DeleteBook", data);
            if (!response.Success)
            {
                TempData["error"] = response.Message;
            }
            TempData["success"] = response.Message;
            return RedirectToAction("BooksList");
        }

        public async Task<IActionResult> GetCategories()
        {
            var bookCategories = await _apiHelper.ApiCall<List<Category>>("values/getcategories");
            if (!bookCategories.Success)
            {
                return NoSuccessJson(bookCategories.Message);
            }
            var result = bookCategories.Result;
            return SuccessJson(result, bookCategories.Message);

        }
        private JsonResult SuccessJson(object? result, string? message = null)
        {
            message ??= "Data fetched successfully.";
            return Json(new { success = true, data = result, message });
        }
        private JsonResult NoSuccessJson(string? message = null)
        {
            message ??= "Data fetched Failed.";
            return Json(new { success = false, message });
        }
    }
}
