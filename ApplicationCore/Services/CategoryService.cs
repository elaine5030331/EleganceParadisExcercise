using ApplicationCore.DTOs.CategoryDTOs;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Microsoft.Extensions.Logging;

namespace ApplicationCore.Services
{
    public class CategoryService : ICategoryService
    {
        private IRepository<Category> _categoryRepo;
        private ILogger<CategoryService> _logger;

        public CategoryService(IRepository<Category> categoryRepo, ILogger<CategoryService> logger)
        {
            _categoryRepo = categoryRepo;
            _logger = logger;
        }

        public async Task<OperationResult> AddCategoryAsync(AddCategoryRequest request)
        {
            try
            {
                var category = new Category
                {
                    Name = request.Name,
                    ImageUrl = request.ImageURL,
                    Description = request.Description,
                    Order = 0,
                    IsDelete = false,
                    ParentCategoryId = request.ParentCategoryId
                };

                await _categoryRepo.AddAsync(category);

                return new OperationResult()
                {
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new OperationResult("商品類別新增失敗");
            }
        }

        public async Task<List<GetCategoriesResponse>> GetCategories()
        {
            var categoryEntity = (await _categoryRepo.ListAsync(c => !c.IsDelete)).OrderBy(c => c.Order).ToList();

            var categories = GetCategories(categoryEntity, null);
            return categories;
        }

        private static List<GetCategoriesResponse> GetCategories(List<Category> categoryEntity, int? parentCategoryId)
        {
            if (categoryEntity == null || categoryEntity.Count < 1) return null;
            var categoryResult = new List<GetCategoriesResponse>();
            var categories = categoryEntity.Where(c => c.ParentCategoryId == parentCategoryId).ToList();

            if (categories.Count < 1) return null;

            foreach (var item in categories)
            {
                var category = new GetCategoriesResponse
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    ImageURL = item.ImageUrl,
                    Order = item.Order,
                    SubCategory = GetCategories(categoryEntity, item.Id)
                };
                categoryResult.Add(category);
            }
            return categoryResult;
        }

        public async Task<OperationResult> UpdateCategoryInfoAsync(UpdateCategoryInfoRequest request)
        {
            try
            {
                var category = await _categoryRepo.GetByIdAsync(request.CategoryId);
                if (category == null)
                    return new OperationResult("找不到對應的商品類別ID");

                if (request.CategoryId == request.ParentCategoryId)
                    return new OperationResult("不可將自己設為父類別");

                category.Name = request.Name;
                category.Description = request.Description;
                category.ImageUrl = request.ImageURL;
                category.ParentCategoryId = request.ParentCategoryId;

                await _categoryRepo.UpdateAsync(category);

                return new OperationResult()
                {
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new OperationResult("商品類別更新失敗");
            }
        }

        public async Task<OperationResult> DeleteCategoryAsync(int categoryId)
        {
            try
            {
                var category = await _categoryRepo.GetByIdAsync(categoryId);
                if (category == null)
                    return new OperationResult("找不到對應的商品類別ID");

                category.IsDelete = true;
                await _categoryRepo.UpdateAsync(category);

                return new OperationResult()
                {
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new OperationResult("商品類別刪除失敗");
            }
        }

        public async Task<OperationResult> UpdateCategoryOrderAsync(int? parentCategoryId, UpdateCategoryOrderRequest request)
        {
            try
            {
                var categories = await _categoryRepo.ListAsync(c => !c.IsDelete);
                if(categories == null)
                    return new OperationResult("目前尚未建立商品類別");

                var subCategories = categories.Where(c => c.ParentCategoryId == parentCategoryId);
                var subCategoryIds = subCategories.Select(c => c.Id);
                var intersectList = request.SubCategoryIdList.Intersect(subCategoryIds);
                if (intersectList.Count() != request.SubCategoryIdList.Count())
                    return new OperationResult("參數異常");

                foreach(var subCategory in subCategories)
                {
                    subCategory.Order = request.SubCategoryIdList.IndexOf(subCategory.Id);
                }
                await _categoryRepo.UpdateRangeAsync(subCategories);

                return new OperationResult();
            }
            catch(Exception ex)
            {
                _logger?.LogError(ex, ex.Message);
                return new OperationResult("更新商品類別順序失敗");
            }
        }
    }
}
