using LibraryManagementSystem.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllCategoriesAsync();
        Task AddCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(int categoryId);
    }
}