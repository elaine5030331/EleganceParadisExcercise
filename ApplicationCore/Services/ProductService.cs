using ApplicationCore.DTOs.ProductDTOs;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Microsoft.Extensions.Logging;

namespace ApplicationCore.Services
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _productRepo;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IRepository<Product> productRepo, ILogger<ProductService> logger)
        {
            _productRepo = productRepo;
            _logger = logger;
        }

        public async Task<OperationResult> AddProductAsync(AddProductDTO addProductDTO)
        {
            var pruduct = new Product
            {
                CategoryId = addProductDTO.CategoryId,
                Spu = addProductDTO.SPU,
                ProductName = addProductDTO.ProductName,
                Enable = false,
                IsDelete = false,
                Description = addProductDTO.Description,
                CreateAt = DateTimeOffset.UtcNow,
                Specs = new List<Spec> 
                {
                    new Spec
                    {
                        Sku = string.Empty,
                        SpecName = string.Empty,
                        CreateAt = DateTimeOffset.UtcNow
                    } 
                }
            };

            try
            {
                await _productRepo.AddAsync(pruduct);
                return new OperationResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new OperationResult("新增失敗");
            }
        }

        public async Task<OperationResult> UpdateProductAsync(int productId, UpdateProductDTO updateProductDTO)
        {
            var product = _productRepo.GetById(productId);
            product.Spu = updateProductDTO.SPU;
            product.ProductName = updateProductDTO.ProductName;
            product.Enable = updateProductDTO.Enable;
            product.Description = updateProductDTO.Description;
            try
            {
                await _productRepo.UpdateAsync(product);
                return new OperationResult();
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex,ex.Message);
                return new OperationResult("更新失敗");
            }
        }

        public async Task<OperationResult> DeleteProductAsync(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            product.IsDelete = true;
            try
            {
                await _productRepo.UpdateAsync(product);
                return new OperationResult();
            }
            catch (Exception ex) 
            { 
                _logger.LogError(ex, ex.Message );
                return new OperationResult("刪除失敗");
            }
        }
    }
}
