using ApplicationCore.DTOs.CategoryDTOs;
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
        private readonly IRepository<Spec> _specRepo;
        private readonly ILogger<ProductService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Category> _categoryRepo;

        public ProductService(IRepository<Product> productRepo, ILogger<ProductService> logger, IRepository<ProductImage> productImageRepo, IUnitOfWork unitOfWork, IRepository<Spec> specRepo, IRepository<Category> categoryRepo)
        {
            _productRepo = productRepo;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _productImageRepo = unitOfWork.GetRepository<ProductImage>();
            _specRepo = specRepo;
            _categoryRepo = categoryRepo;
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
            try
            {
                var product = _productRepo.GetById(productId);
                if (product == null) return new OperationResult("找不到對應的商品");

                product.CategoryId = updateProductDTO.CategoryId;
                product.Spu = updateProductDTO.SPU;
                product.ProductName = updateProductDTO.ProductName;
                product.Enable = updateProductDTO.Enable;
                product.Description = updateProductDTO.Description;

                await _productRepo.UpdateAsync(product);
                return new OperationResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new OperationResult("更新商品資料失敗");
            }
        }

        public async Task<OperationResult> DeleteProductAsync(int id)
        {
            try
            {
                var product = await _productRepo.GetByIdAsync(id);
                if (product == null) return new OperationResult("找不到對應的商品");

                product.IsDelete = true;
                await _productRepo.UpdateAsync(product);
                return new OperationResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new OperationResult("刪除商品失敗");
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
            catch (Exception ex)
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

        public async Task<OperationResult<GetAllProductsResponse>> GetAllProductsAsync(GetAllProductsRequest request)
        {
            try
            {
                List<Product> products = new List<Product>();

                if (request.CategoryId != null)
                {
                    products = await _productRepo.ListAsync(p => p.CategoryId == request.CategoryId && !p.IsDelete);
                }
                else
                {
                    products = await _productRepo.ListAsync(p => !p.IsDelete);
                }

                if (products.Count < 1)
                    return new OperationResult<GetAllProductsResponse>("目前尚未有商品");

                products = products.OrderBy(p => p.Order).ToList();
                var categories = await _categoryRepo.ListAsync(c => products.Select(p => p.CategoryId).Contains(c.Id));
                var specs = (await _specRepo.ListAsync(s => products.Select(p => p.Id).Contains(s.ProductId))).OrderBy(s => s.Order);
                var productImages = (await _productImageRepo.ListAsync(pi => products.Select(p => p.Id).Contains(pi.ProductId))).OrderBy(pi => pi.Order);

                return new OperationResult<GetAllProductsResponse>()
                {
                    IsSuccess = true,
                    ResultDTO = new GetAllProductsResponse()
                    {
                        ProductList = products.Select(p => new ProductItem
                        {
                            CategoryId = p.CategoryId,
                            CategoryName = categories.FirstOrDefault(c => c.Id == p.CategoryId)?.Name ?? string.Empty,
                            ProductId = p.Id,
                            ProductName = p.ProductName,
                            SPU = p.Spu,
                            Description = p.Description,
                            Enable = p.Enable,
                            CreateAt = p.CreateAt.ToLocalTime().ToString("yyyy/MM/dd"),
                            SpecList = specs.Where(s => s.ProductId == p.Id).Select(s => new SpecItems
                            {
                                SpecId = s.Id,
                                SKU = s.Sku,
                                SpecName = s.SpecName,
                                UnitPrice = s.UnitPrice,
                                StockQuantity = s.StockQuantity,
                            }).ToList(),
                            ImageList = productImages.Where(pi => pi.ProductId == p.Id).Select(i => new Images
                            {
                                ProductImageId = i.Id,
                                URL = i.Url
                            }).ToList()
                        }).ToList()
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new OperationResult<GetAllProductsResponse>("取得商品清單失敗");
            }
        }

        public async Task<List<GetProductListDTO>> GetProducts(int categoryId)
        {
            var categoryEntities = await _categoryRepo.ListAsync(c => !c.IsDelete);
            var category = categoryEntities.FirstOrDefault(c => c.Id == categoryId);
            if (category == null) return new List<GetProductListDTO>();

            var categoryIdList = new List<int>();
            var temp = new List<int>();

            categoryIdList.Add(category.Id);
            temp.Add(category.Id);
            while (temp.Count > 0)
            {
                var subCategoryIds = categoryEntities.Where(c => c.ParentCategoryId != null && temp.Contains(c.ParentCategoryId.Value)).Select(c => c.Id).ToList();
                categoryIdList.AddRange(subCategoryIds);
                temp = subCategoryIds;
            }

            var products = (await _productRepo.ListAsync(p => categoryIdList.Contains(p.CategoryId) && p.Enable && !p.IsDelete));

            var productImages = (await _productImageRepo.ListAsync(pi => products.Select(p => p.Id).Contains(pi.ProductId)))
                                                        .OrderBy(pi => pi.Order);

            var specs = (await _specRepo.ListAsync(s => products.Select(p => p.Id).Contains(s.ProductId)))
                        .OrderBy(s => s.Order)
                        .ThenBy(s => s.UnitPrice);

            var result = products.OrderBy(p => p.Order)
                                 .Select(p =>
                                 {
                                     var productSpec = specs.Where(s => s.ProductId == p.Id).FirstOrDefault();
                                     return new GetProductListDTO
                                     {
                                         CategoryId = p.CategoryId,
                                         ProductId = p.Id,
                                         CategoryName = categoryEntities.FirstOrDefault(c => c.Id == p.CategoryId)?.Name ?? "預設分類",
                                         ProductName = p.ProductName,
                                         UnitPrice = productSpec?.UnitPrice ?? -1,
                                         ProductImageUrl = productImages.FirstOrDefault(pi => pi.ProductId == p.Id)?.Url ?? "https://eleganceparadisapp.azurewebsites.net/images/item_1.webp"
                                     };
                                 });
            return result.ToList();
        }

        public async Task<OperationResult> UpdateProductOrderAsync(UpdateProductOrderRequest request)
        {
            try
            {
                var products = await _productRepo.ListAsync(p => !p.IsDelete);
                if (products == null) return new OperationResult("目前尚未建立商品資料");

                var productIds = products.Select(p => p.Id).ToList();
                var intersectList = request.ProductIdList.Intersect(productIds).ToList();
                if (productIds.Count != intersectList.Count) return new OperationResult("參數異常");

                foreach (var product in products)
                {
                    product.Order = request.ProductIdList.IndexOf(product.Id);
                }

                await _productRepo.UpdateRangeAsync(products);
                return new OperationResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new OperationResult("更新商品順序失敗");
            }
        }
    }
}
