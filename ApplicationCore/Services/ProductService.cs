using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using static ApplicationCore.Interfaces.DTOs.ProductEditDTO;

namespace ApplicationCore.Services
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _productRepo;

        public ProductService(IRepository<Product> productRepo)
        {
            _productRepo = productRepo;
        }

        public async Task<Product> AddProductAsync(AddProductDTO addProductDTO)
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

            return await _productRepo.AddAsync(pruduct);
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
