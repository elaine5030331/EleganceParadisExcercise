using ApplicationCore.DTOs.ProductDTOs;
using ApplicationCore.Entities;
using ApplicationCore.Models;

namespace ApplicationCore.Interfaces
{
    public interface IProductService
    {
        Task<OperationResult> AddProductAsync(AddProductDTO product);
        Task<OperationResult> UpdateProductAsync(int productId, UpdateProductDTO updateProductDTO);
        Task DeleteProductAsync(int id);
    }
}
