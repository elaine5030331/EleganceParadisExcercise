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
        private readonly IRepository<ProductImage> _productImageRepo;
        private readonly ILogger<ProductService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IRepository<Product> productRepo, ILogger<ProductService> logger, IRepository<ProductImage> productImageRepo, IUnitOfWork unitOfWork)
        {
            _productRepo = productRepo;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _productImageRepo = unitOfWork.GetRepository<ProductImage>();
        }

        public async Task<OperationResult<AddProductResponse>> AddProductAsync(AddProductDTO addProductDTO)
        {
            var productImages = addProductDTO.ProductImageList?.Select((url, index) => new ProductImage
            {
                Order = index,
                Url = url
            }).ToList();

            var product = new Product
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
                },
                ProductImages = productImages
            };

            try
            {
                await _productRepo.AddAsync(product);
                return new OperationResult<AddProductResponse>
                {
                    IsSuccess = true,
                    ResultDTO = new AddProductResponse
                    {
                        ProductId = product.Id
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new OperationResult<AddProductResponse>("新增失敗");
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
                _logger.LogError(ex, ex.Message);
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
                _logger.LogError(ex, ex.Message);
                return new OperationResult("刪除失敗");
            }
        }

        public async Task<OperationResult> AddProductImagesAsync(int productId, List<string> imageUrlList)
        {
            try
            {
                var productImages = imageUrlList.Select((url, index) => new ProductImage
                {
                    ProductId = productId,
                    Order = index,
                    Url = url
                }).ToList();

                await _productImageRepo.AddRangeAsync(productImages);

                return new OperationResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new OperationResult("商品圖片新增失敗");
            }

        }

        public async Task<OperationResult<UpdateProductImagesResponse>> UpdateProductImagesAsync(int productId, List<string> imageUrlList)
        {
            try
            {
                var productImageList = await _productImageRepo.ListAsync(pi => pi.ProductId == productId);
                if (productImageList.Count < 1) return new OperationResult<UpdateProductImagesResponse>("找不到對應的商品圖");

                await _unitOfWork.BeginAsync();
                await _productImageRepo.DeleteRangeAsync(productImageList);

                var result = await AddProductImagesAsync(productId, imageUrlList);
                await _unitOfWork.CommitAsync();
                if (result.IsSuccess)
                {
                    return new OperationResult<UpdateProductImagesResponse>
                    {
                        IsSuccess = true,
                        ResultDTO = new UpdateProductImagesResponse
                        {
                            ProductId = productId,
                            ImageUrlList = imageUrlList
                        }
                    };
                }

                return new OperationResult<UpdateProductImagesResponse>("商品圖片更新失敗");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await _unitOfWork.RollbackAsync();
                return new OperationResult<UpdateProductImagesResponse>("商品圖片更新失敗");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }
    }
}
