using ApplicationCore.DTOs.CategoryDTOs;
using ApplicationCore.Models;

namespace ApplicationCore.Interfaces
{
    public interface ICategoryService
    {
        Task<List<GetCategoriesResponse>> GetCategories();
        Task<OperationResult> AddCategoryAsync(AddCategoryRequest request);
        Task<OperationResult> DeleteCategoryAsync(int categoryId);
        Task<OperationResult> UpdateCategoryInfoAsync(UpdateCategoryInfoRequest request);
        Task<OperationResult> UpdateCategoryOrderAsync(int? parentCategoryId, UpdateCategoryOrderRequest request);
    }
}