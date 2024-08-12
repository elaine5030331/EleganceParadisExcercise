using ApplicationCore.Interfaces;
using EleganceParadisAPI.Services;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.DTOs.ProductDTOs;

namespace EleganceParadisAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductQueryService _productQueryService;
        private readonly IProductService _productService;

        public ProductController(IProductQueryService productQueryService, IProductService productService)
        {
            _productQueryService = productQueryService;
            _productService = productService;
        }

        /// <summary>
        /// 新增產品
        /// </summary>
        /// <param name="addProductDTO"></param>
        /// <returns></returns>
        /// <remarks>
        ///Sample request:<br/>
        ///     {
        ///        "categoryId": 類別ID(ex：香水、隨身噴香霧、沐浴與身體保養),
        ///        "spu": "產品編號",
        ///        "productName": "產品名稱",
        ///        "description": "產品描述",
        ///        "productImageList": ["string"]
        ///     }
        /// </remarks>
        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct(AddProductDTO addProductDTO)
        {
            var result = await _productService.AddProductAsync(addProductDTO);
            if (result.IsSuccess) return Ok(result.ResultDTO);
            return BadRequest(result.ErrorMessage);
        }

        /// <summary>
        /// 取得商品列表資訊，價格會顯示此產品中規格價位最低者
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        [HttpGet("GetProductsByCategory/{categoryId}")]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            var result = await _productQueryService.GetProducts(categoryId);
            return Ok(result);
        }

        /// <summary>
        /// 商品資料
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        /// <response code ="404">找不到此產品</response>
        [HttpGet("GetProductById/{productId}")]
        public async Task<IActionResult> GetProductById(int productId)
        {
            var result = await _productQueryService.GetProductById(productId);
            if (result == null) return StatusCode(404);
            return Ok(result);
        }

        /// <summary>
        /// 更新產品資料
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="updateProductDTO"></param>
        /// <returns></returns>
        /// <response code ="204">更新成功</response>
        /// <response code ="400">更新失敗</response>
        [HttpPut("UpdateProduct/{productId}")]
        public async Task<IActionResult> UpdateProduct(int productId, UpdateProductDTO updateProductDTO)
        {
            if (productId != updateProductDTO.ProductId) return BadRequest();
            var result = await _productService.UpdateProductAsync(productId, updateProductDTO);
            if (result.IsSuccess) return NoContent();
            return BadRequest(result.ErrorMessage);
        }

        /// <summary>
        /// 刪除產品
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        /// <response code ="204">更新成功</response>
        /// <response code ="400">更新失敗</response>
        [HttpDelete("DeleteProduct/{productId}")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var result = await _productService.DeleteProductAsync(productId);
            if (result.IsSuccess) return NoContent();
            return BadRequest(result.ErrorMessage);
        }
    }
}
