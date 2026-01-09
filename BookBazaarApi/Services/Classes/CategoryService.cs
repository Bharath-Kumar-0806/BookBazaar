using BookBazaarApi.Models;
using BookBazaarApi.Repos.Interfaces;
using BookBazaarApi.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookBazaarApi.Services.Classes
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _categoryRepository.GetAllAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            return await _categoryRepository.GetByIdAsync(id);
        }

        public async Task CreateCategoryAsync(Category category)
        {
            await _categoryRepository.AddAsync(category);
        }

        public async Task<bool> UpdateCategoryAsync(Category category)
        {
            var existingCategory = await _categoryRepository.GetByIdAsync(category.Id);
            if (existingCategory == null)
                return false;

            existingCategory.Name = category.Name;
            await _categoryRepository.UpdateAsync(existingCategory);
            return true;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var existingCategory = await _categoryRepository.GetByIdAsync(id);
            if (existingCategory == null)
                return false;

            await _categoryRepository.DeleteAsync(id);
            return true;
        }
    }
}
