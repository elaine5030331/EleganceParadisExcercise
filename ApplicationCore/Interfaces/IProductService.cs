using ApplicationCore.Entities;
using static ApplicationCore.Interfaces.DTOs.ProductEditDTO;

namespace ApplicationCore.Interfaces
{
    public interface IProductService
    {
        Task<Product> AddProductAsync(AddProductDTO product);
        Task<Product> UpdateProductAsync(int productId, UpdateProductDTO updateProductDTO);
        Task DeleteProductAsync(int id);
    }
}
