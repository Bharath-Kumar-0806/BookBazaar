using BookBazaarApi.Models;

namespace BookBazaarApi.Repos.Interfaces
{
    public interface ICategory
    {
        Task<bool> IsValidCategoryAsync(Category category);
    }
}
