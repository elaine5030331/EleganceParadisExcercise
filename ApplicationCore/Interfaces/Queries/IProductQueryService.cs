using ApplicationCore.DTOs.ProductDTOs;

namespace EleganceParadisAPI.Services
{
    public interface IProductQueryService
    {
        Task<ProductDTO> GetProductById(int productId);
        Task<List<GetProductListDTO>> GetProducts(int categoryId);
    }
}