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
            var parentCategory = (await _categoryRepo.ListAsync(x => x.ParentCategoryId == null && !x.IsDelete)).OrderBy(x => x.Order);
            var childrenCategory = await _categoryRepo.ListAsync(c => !c.IsDelete && parentCategory.Select(pc => pc.Id).Contains(c.ParentCategoryId.Value));
            return parentCategory.Select(pc => new GetCategoriesResponse
            {
                Id = pc.Id,
                Description = pc.Description,
                ImageURL = pc.ImageUrl,
                Name = pc.Name,
                Order = pc.Order,
                SubCategory = childrenCategory.Where(x => x.ParentCategoryId.Value == pc.Id).Select(c => new GetCategoriesResponse
                {
                    Id = c.Id,
                    Description = c.Description,
                    ImageURL = c.ImageUrl,
                    Name = c.Name,
                    Order = c.Order
                }).ToList()
            }).ToList();
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
    }
}
