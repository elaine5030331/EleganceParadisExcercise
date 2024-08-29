using ApplicationCore.Interfaces;
using EleganceParadisAPI.Services;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.DTOs.ProductDTOs;
using EleganceParadisAPI.Helpers;
using EleganceParadisAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using ApplicationCore.Constants;
using ApplicationCore.DTOs.CategoryDTOs;

namespace EleganceParadisAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductQueryService _productQueryService;
        private readonly IProductService _productService;
        private readonly IUploadImageService _imageService;

        public ProductController(IProductQueryService productQueryService, IProductService productService, IUploadImageService imageService)
        {
            _productQueryService = productQueryService;
            _productService = productService;
            _imageService = imageService;
        }

        /// <summary>
        /// 取得商品列表資訊，價格會顯示此產品中規格價位最低者
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        [HttpGet("GetProductsByCategory/{categoryId}")]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            var result = await _productService.GetProducts(categoryId);
            return Ok(result);
        }

        /// <summary>
        /// 取得商品資料By productId
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
    }
}
