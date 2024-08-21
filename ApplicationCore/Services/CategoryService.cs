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
                var isCategoryExist = await _categoryRepo.AnyAsync(c => c.Name == request.Name);
                if (isCategoryExist)
                    return new OperationResult("該商品類別名稱已存在");

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
            var parentCategory = (await _categoryRepo.ListAsync(x => x.ParentCategoryId == null)).OrderBy(x => x.Order);
            var childrenCategory = await _categoryRepo.ListAsync(c => parentCategory.Select(pc => pc.Id).Contains(c.ParentCategoryId.Value));
            return parentCategory.Where(pc => !pc.IsDelete).Select(pc => new GetCategoriesResponse
            {
                Id = pc.Id,
                Description = pc.Description,
                ImageURL = pc.ImageUrl,
                Name = pc.Name,
                Order = pc.Order,
                SubCategory = childrenCategory.Where(x => x.ParentCategoryId.Value == pc.Id && !x.IsDelete).Select(c => new GetCategoriesResponse
                {
                    Id = c.Id,
                    Description = c.Description,
                    ImageURL = c.ImageUrl,
                    Name = c.Name,
                    Order = c.Order
                }).ToList()
            }).ToList();
        }
    }
}
