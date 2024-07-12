using EleganceParadisAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace EleganceParadisAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductQueryService _productQueryService;

        public ProductController(IProductQueryService productQueryService)
        {
            _productQueryService = productQueryService;
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
    }
}
