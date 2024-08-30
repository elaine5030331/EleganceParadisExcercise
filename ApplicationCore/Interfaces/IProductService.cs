using ApplicationCore.DTOs.CategoryDTOs;
using ApplicationCore.DTOs.ProductDTOs;
using ApplicationCore.Entities;
using ApplicationCore.Models;

namespace ApplicationCore.Interfaces
{
    public interface IProductService
    {
        Task<OperationResult<AddProductResponse>> AddProductAsync(AddProductDTO product);
        Task<OperationResult> UpdateProductAsync(int productId, UpdateProductDTO updateProductDTO);
        Task<OperationResult> DeleteProductAsync(int id);
        Task<OperationResult> AddProductImagesAsync(int productId, List<string> imageUrlList);
        Task<OperationResult<UpdateProductImagesResponse>> UpdateProductImagesAsync(int productId, List<string> imageUrlList);
        Task<OperationResult<GetAllProductsResponse>> GetAllProductsAsync(GetAllProductsRequest request);
        Task<List<GetProductListDTO>> GetProducts(int categoryId);
        Task<OperationResult> UpdateProductOrderAsync(UpdateProductOrderRequest request);
    }
}
