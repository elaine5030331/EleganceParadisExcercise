using ApplicationCore.Interfaces.DTOs;

namespace ApplicationCore.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryDTO>> GetCategories();
    }
}