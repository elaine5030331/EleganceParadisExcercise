﻿using ApplicationCore.DTOs;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;

namespace ApplicationCore.Services
{
    public class CategoryService : ICategoryService
    {
        private IRepository<Category> _categoryRepo;

        public CategoryService(IRepository<Category> categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        public async Task<List<CategoryDTO>> GetCategories()
        {
            var parentCategory = (await _categoryRepo.ListAsync(x => x.ParentCategoryId == null)).OrderBy(x => x.Order);
            var childrenCategory = await _categoryRepo.ListAsync(c => parentCategory.Select(pc => pc.Id).Contains(c.ParentCategoryId.Value));
            return parentCategory.Where(pc => !pc.IsDelete).Select(pc => new CategoryDTO
            {
                Id = pc.Id,
                Description = pc.Description,
                ImageURL = pc.ImageUrl,
                Name = pc.Name,
                Order = pc.Order,
                SubCategory = childrenCategory.Where(x => x.ParentCategoryId.Value == pc.Id && !x.IsDelete).Select(c => new CategoryDTO
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
