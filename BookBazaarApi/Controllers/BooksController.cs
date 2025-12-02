using BookBazaarApi.Helpers;
using BookBazaarApi.Models;
using BookBazaarApi.Services.Interfaces;
using BookBazaarApi.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BookBazaarApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }
        [HttpGet("GetBooks")]
        public async Task<ActionResult<ResponseModel<List<Book>>>> GetBooks()
        {
            var result = await _bookService.GetAllBooksAsync();
            var response = new ResponseModel<List<Book>>
            {
                Success = true,
                Result= result
            };
            return Ok(response);
        }
        [HttpPost("SearchBook")]
        public async Task<ActionResult<ResponseModel<List<Book>>>> SearchBook(RequestModel data)
        {
            var result = await _bookService.SearchBookAsync(data);
            var response = new ResponseModel<List<Book>>
            {
                Success = true,
                Result = result
            };
            return Ok(response);
        }

        [HttpPost("GetByIdAsync")]
        public async Task<ActionResult<ResponseModel<Book>>> GetByIdAsync(RequestModel data)
        {
            var result = await _bookService.GetBookByIdAsync(data.Id);

            var response = new ResponseModel<Book>
            {
                Success = true,
                Result = result,
                Message = "Category retrieved successfully"
            };

            return Ok(response);
        }


        [HttpPost("SaveBook")]
        public ActionResult<ResponseModel<Book>> SaveBook(Book book) 
        {
            var result = _bookService.CreateBookAsync(book);
            var response = new ResponseModel<Book>
            {
                Success = true,
                Message = "Book saved successfully."
            };
            return Ok(response);
        }
        [HttpPost("EditBook")]
        public async Task<ActionResult<ResponseModel<Book>>> EditBook(Book obj)
        {
            var result = await _bookService.UpdateBookAsync(obj);
            var response = new ResponseModel<Book>
            {
                Success = true,
                Message = "Book saved successfully."
            };
            return Ok(response);
            

        }
        [HttpPost("DeleteBook")]
        public async Task<ActionResult<ResponseModel<object>>> DeleteBook(RequestModel data)
        {
            var result = await _bookService.DeleteBookAsync(data.Id);

            var response = new ResponseModel<object>
            {
                Success = result,
                Message = result ? "Book deleted successfully." : "Book not found or could not be deleted.",
                Result = null
            };

            return Ok(response);
        }
        [HttpPost("GetBestSellersViewAll")]
        public async Task<ActionResult<ResponseModel<List<BookVM>>>> GetBestSellersViewAll(RequestModel data)
        {
            var result = await _bookService.GetBestSellersViewAll(data);
            var response = new ResponseModel<List<BookVM>>
            {
                Success = true,
                Result = result
            };
            return Ok(response);
        }
        [HttpPost("GetNewArrivalsViewAll")]
        public async Task<ActionResult<ResponseModel<List<BookVM>>>> GetNewArrivalsViewAll(RequestModel data)
        {
            var result = await _bookService.GetNewArrivalsViewAll(data);
            var response = new ResponseModel<List<BookVM>>
            {
                Success = true,
                Result = result
            };
            return Ok(response);
        }
        [HttpPost("HomePageBooks")]
        public async Task<ActionResult<ResponseModel<UserHomeVM>>> HomePageBooks(RequestModel data)
        {
            var result = await _bookService.HomePageBooks(data);
            var response = new ResponseModel<UserHomeVM>
            {
                Success = true,
                Result = result
            };
            return Ok(response);
        }

        [HttpPost("GetBookDetails")]
        public async Task<ActionResult<ResponseModel<BookDetailsVM>>> GetBookDetails(RequestModel data)
        {
            var result = await _bookService.GetBookDetailsAsync(data.Id);

            var response = new ResponseModel<BookDetailsVM>
            {
                Success = true,
                Result = result
                //Message = "Category retrieved successfully"
            };

            return Ok(response);
        }

        [HttpPost("GetBooksByCategoryId")]
        public async Task<ActionResult<ResponseModel<List<BookVM>>>> GetBooksByCategoryId(RequestModel data)
        {
            var result = await _bookService.GetBooksByCategoryId(data);
            var response = new ResponseModel<List<BookVM>>
            {
                Success = true,
                Result = result
            };
            return Ok(response);
        }
        [HttpPost("SearchResults")]
        public async Task<ActionResult<ResponseModel<List<BookVM>>>> SearchResults(RequestModel data)
        {
            var result = await _bookService.SearchResults(data);
            var response = new ResponseModel<List<BookVM>>
            {
                Success = true,
                Result = result,
                Message = data.Key
            };
            return Ok(response);
        }
    }
}
