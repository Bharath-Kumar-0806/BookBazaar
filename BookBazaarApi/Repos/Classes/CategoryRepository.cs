using BookBazaarApi.DAL;
using BookBazaarApi.Models;
using BookBazaarApi.Repos.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.EntityFrameworkCore;

namespace BookBazaarApi.Repos.Classes
{
    public class CategoryRepository : ICategory
    {
        private readonly AppDbContext _dal;

        public CategoryRepository(AppDbContext appDbContext)
        {
            _dal = appDbContext;
        }
        public async Task<bool> IsValidCategoryAsync(Category category)
        {
            return await _dal.Categories.AnyAsync(c => c.Id == category.Id && c.Name == category.Name);
        }
    }
}
