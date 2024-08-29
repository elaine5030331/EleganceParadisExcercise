using ApplicationCore.Constants;
using ApplicationCore.DTOs.CategoryDTOs;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EleganceParadisAPI.AdminControllers
{
    [Route("api/AdminCategoryController")]
    [ApiController]
    [Authorize(Roles = EleganceParadisRole.Admin)]
    public class AdminCategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public AdminCategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// 取得所有商品類別
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllCategory")]
        public async Task<IActionResult> GetAllCategory()
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
        /// <response code ="200">商品類別新增成功</response>
        /// <response code ="400">商品類別新增失敗</response>
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
        /// <response code ="200">商品類別刪除失敗</response>
        /// <response code ="400">
        /// 1. 找不到對應的商品類別ID
        /// 2. 商品類別刪除失敗
        /// </response>
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
        /// <response code ="200">商品類別更新成功</response>
        /// <response code ="400">
        /// 1. 參數有問題
        /// 2. 找不到對應的商品類別ID
        /// 3. 不可將自己設為父類別
        /// 4. 商品類別更新失敗
        /// </response>
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
