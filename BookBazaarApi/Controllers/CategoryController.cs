using BookBazaarApi.DAL;
using BookBazaarApi.Helpers;
using BookBazaarApi.Models;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookBazaarApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _dal;

        public CategoryController(AppDbContext appDbContext)
        {
            _dal = appDbContext;
        }


        [HttpPost("GetCategoryById")]
        public ActionResult<ResponseModel<Category>> GetCategoryById(RequestModel data)
        {
            var result = _dal.Categories.FirstOrDefault(c => c.Id == data.Id);

            var response = new ResponseModel<Category>
            {
                Success = true,
                Result = result,
                Message = "Category retrieved successfully"
            };

            return Ok(response);
        }

        [HttpGet("GetCategories")]
        public ActionResult<ResponseModel<List<Category>>> GetCategories()
        {
            var result = _dal.Categories.ToList();

            var response = new ResponseModel<List<Category>>
            {
                Success = true,
                Result = result,
                Message = "Categories retrieved successfully"
            };

            return Ok(response);
        }

        [HttpPost("SaveCategory")]
        public ActionResult<ResponseModel<List<Category>>> SaveCategory(Category obj)
        {
            try
            {
                _dal.Categories.Add(obj);
                _dal.SaveChanges();

                var response = new ResponseModel<List<Category>>
                {
                    Success = true,
                    Message = "Category saved successfully."
                };
                return Ok(response);
            }
            catch
            {
                var response = new ResponseModel<List<Category>>
                {
                    Success = false,
                    Message = "Category did not saved."
                };
                return Ok(response);
            }

         
        }
        [HttpPost("EditCategory")]
        public ActionResult<ResponseModel<List<Category>>> EditCategory(Category obj)
        {
            var categoryData = _dal.Categories.FirstOrDefault(c => c.Id == obj.Id);
            if(categoryData == null)
            {
                var response = new ResponseModel<List<Category>>
                {
                    Success = false,
                    Message = "Invalid Category"
                };
                return Ok(response);

            }
            else
            {
                categoryData.Name = obj.Name;
                _dal.SaveChanges();

                var response = new ResponseModel<List<Category>>
                {
                    Success = true,
                    Message = "Category edited successfully."
                };
                return Ok(response);
            }

        }
        [HttpPost("DeleteCategory")]
        public ActionResult<ResponseModel<object>> DeleteCategory(RequestModel data)
        {
         
            var categoryData = _dal.Categories.FirstOrDefault(c => c.Id == data.Id);
            if (categoryData == null)
            {
                    var response = new ResponseModel<object>
                    {
                        Success = false,
                        Message = "Invalid Category."
                    };
                    return Ok(response);
            }
            else
            {
                    _dal.Categories.Remove(categoryData);
                    _dal.SaveChanges();

                    var response = new ResponseModel<object>
                    {
                        Success = true,
                        Message = "Category deleted successfully."
                    };
                    return Ok(response);
            }
        }

    }
}
