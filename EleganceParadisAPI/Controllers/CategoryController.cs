using ApplicationCore.Constants;
using ApplicationCore.DTOs.CategoryDTOs;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EleganceParadisAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = EleganceParadisRole.Admin)]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllCategory()
        {
            var result = await _categoryService.GetCategories();
            return Ok(result);
        }

        [HttpGet("AdminGetAllCategory")]
        public async Task<IActionResult> AdminGetAllCategory()
        {
            var result = await _categoryService.GetCategories();
            return Ok(result);
        }

        /// <summary>
        /// 新增商品類別
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:<br/>
        ///     {
        ///      "name": "類別名稱",
        ///      "imageURL": "類別圖片URL",
        ///      "description": "類別描述",
        ///      "parentCategoryId": 父類別ID
        ///     }
        /// </remarks>
        [HttpPost("AddCategory")]
        public async Task<IActionResult> AddCategory(AddCategoryRequest request)
        {
            var result = await _categoryService.AddCategoryAsync(request);
            if (result.IsSuccess) return Ok();
            return BadRequest(result.ErrorMessage);
        }

        /// <summary>
        /// 刪除商品類別
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        [HttpDelete("DeleteCategory/{categoryId}")]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            var result = await _categoryService.DeleteCategoryAsync(categoryId);
            if (result.IsSuccess) return Ok();
            return BadRequest(result.ErrorMessage);
        }

        /// <summary>
        /// 更新商品類別
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("UpdateCategoryInfo/{categoryId}")]
        public async Task<IActionResult> UpdateCategoryInfo(int categoryId, UpdateCategoryInfoRequest request)
        {
            if (categoryId != request.CategoryId)
                return BadRequest("參數有問題");

            var result = await _categoryService.UpdateCategoryInfoAsync(request);
            if (result.IsSuccess) return Ok();
            return BadRequest(result.ErrorMessage);
        }
    }
}
