using ApplicationCore.DTOs.CategoryDTOs;
using ApplicationCore.Models;

namespace ApplicationCore.Interfaces
{
    public interface ICategoryService
    {
        Task<List<GetCategoriesResponse>> GetCategories();
        Task<OperationResult> AddCategoryAsync(AddCategoryRequest request);
        Task<OperationResult> DeleteCategoryAsync(int categoryId);
    }
}