using ApplicationCore.Interfaces.DTOs;

namespace EleganceParadisAPI.Services
{
    public interface IProductQueryService
    {
        Task<ProductDTO> GetProductById(int productId);
        Task<List<ProductListDTO>> GetProducts(int categoryId);
    }
}