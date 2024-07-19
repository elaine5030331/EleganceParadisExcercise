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
            var productList = await _productRepo.ListAsync(x => x.CategoryId == addProductDTO.CategoryId);
            var lastProduct = productList.OrderByDescending(p => p.Order).FirstOrDefault();
            var lastOrder = lastProduct == null ? 0 : (lastProduct.Order + 1);
            var pruduct = new Product
            {
                CategoryId = addProductDTO.CategoryId,
                Spu = addProductDTO.SPU,
                ProductName = addProductDTO.ProductName,
                Enable = addProductDTO.Enable,
                IsDelete = false,
                Order = lastOrder,
                Description = addProductDTO.Description
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

        public async Task<Product> UpdateProductAsync(int productId, UpdateProductDTO updateProductDTO)
        {
            var product = _productRepo.GetById(productId);
            product.Spu = updateProductDTO.SPU;
            product.ProductName = updateProductDTO.ProductName;
            product.Enable = updateProductDTO.Enable;
            product.Order = updateProductDTO.Order;
            product.Description = updateProductDTO.Description;
            return await _productRepo.UpdateAsync(product);
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            product.IsDelete = true;
            await _productRepo.UpdateAsync(product);
        }
    }
}
