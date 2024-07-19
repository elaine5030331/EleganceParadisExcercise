using ApplicationCore.DTOs.ProductDTOs;
using ApplicationCore.Entities;

namespace ApplicationCore.Interfaces
{
    public interface IProductService
    {
        Task<Product> AddProductAsync(AddProductDTO product);
        Task<Product> UpdateProductAsync(int productId, UpdateProductDTO updateProductDTO);
        Task DeleteProductAsync(int id);
    }
}
