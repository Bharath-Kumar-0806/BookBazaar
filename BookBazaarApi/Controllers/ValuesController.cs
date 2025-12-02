using Azure;
using BookBazaarApi.DAL;
using BookBazaarApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using BookBazaarApi.Helpers;

namespace BookBazaarApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly AppDbContext _dal;

        public ValuesController(AppDbContext appDbContext)
        {
            _dal = appDbContext;
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
    }
}
