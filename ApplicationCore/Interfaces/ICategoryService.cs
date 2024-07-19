using ApplicationCore.DTOs;

namespace ApplicationCore.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryDTO>> GetCategories();
    }
}