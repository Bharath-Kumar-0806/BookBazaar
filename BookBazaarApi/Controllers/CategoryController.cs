using BookBazaarApi.Helpers;
using BookBazaarApi.Models;
using BookBazaarApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookBazaarApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("GetCategories")]
        public async Task<ActionResult<ResponseModel<IEnumerable<Category>>>> GetCategories()
        {
            var result = await _categoryService.GetAllCategoriesAsync();
            var response = new ResponseModel<IEnumerable<Category>>
            {
                Success = true,
                Result = result,
                Message = "Categories retrieved successfully"
            };
            return Ok(response);
        }

        [HttpPost("GetCategoryById")]
        public async Task<ActionResult<ResponseModel<Category>>> GetCategoryById(RequestModel data)
        {
            var result = await _categoryService.GetCategoryByIdAsync(data.Id);
            if (result == null)
            {
                return NotFound(new ResponseModel<Category> { Success = false, Message = "Category not found" });
            }
            var response = new ResponseModel<Category>
            {
                Success = true,
                Result = result,
                Message = "Category retrieved successfully"
            };
            return Ok(response);
        }

        [HttpPost("SaveCategory")]
        public async Task<ActionResult<ResponseModel<Category>>> SaveCategory(Category obj)
        {
            await _categoryService.CreateCategoryAsync(obj);
            var response = new ResponseModel<Category>
            {
                Success = true,
                Message = "Category saved successfully."
            };
            return Ok(response);
        }

        [HttpPost("EditCategory")]
        public async Task<ActionResult<ResponseModel<Category>>> EditCategory(Category obj)
        {
            var result = await _categoryService.UpdateCategoryAsync(obj);
            if (!result)
            {
                return NotFound(new ResponseModel<Category> { Success = false, Message = "Category not found" });
            }
            var response = new ResponseModel<Category>
            {
                Success = true,
                Message = "Category edited successfully."
            };
            return Ok(response);
        }

        [HttpPost("DeleteCategory")]
        public async Task<ActionResult<ResponseModel<object>>> DeleteCategory(RequestModel data)
        {
            var result = await _categoryService.DeleteCategoryAsync(data.Id);
            if (!result)
            {
                return NotFound(new ResponseModel<object> { Success = false, Message = "Category not found" });
            }
            var response = new ResponseModel<object>
            {
                Success = true,
                Message = "Category deleted successfully."
            };
            return Ok(response);
        }
    }
}

