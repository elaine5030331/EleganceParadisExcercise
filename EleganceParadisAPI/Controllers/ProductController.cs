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
    [Authorize(Roles = EleganceParadisRole.Admin)]
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
        [AllowAnonymous]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            var result = await _productService.GetProducts(categoryId);
            return Ok(result);
        }

        /// <summary>
        /// 商品資料
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        /// <response code ="404">找不到此產品</response>
        [HttpGet("GetProductById/{productId}")]
        [AllowAnonymous]
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

        /// <summary>
        /// 新增商品圖片
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code ="200">更新成功</response>
        /// <response code ="400">更新失敗</response>
        [HttpPost("AddProductImages")]
        public async Task<IActionResult> AddProductImages([FromForm] AddProductImagesRequest request)
        {
            var files = request.Files;
            if (files == null || files.Count == 0 || files.Any(x => !ImageFileValidator.IsValidateExtensions(x.FileName)))
            {
                return BadRequest("檔案上傳格式有誤");
            }
            var urlList = new List<string>();
            foreach (var file in files)
            {
                var uploadResult = await _imageService.UploadImageAsync(file);
                if (uploadResult.IsSuccess) urlList.Add(uploadResult.ResultDTO.URL);
            }
            var result = await _productService.AddProductImagesAsync(request.ProductId, urlList);
            if (result.IsSuccess) return Ok();
            return BadRequest(result.ErrorMessage);
        }

        /// <summary>
        /// 更新商品圖片
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <remarks>
        ///Sample request:<br/>
        ///     {
        ///         "productId": 5,
        ///         "imageUrlList": 陣列裡放圖片URL，URL存放順序為商品圖顯示順序
        ///         [
        ///           "https://th.bing.com/th/id/R.4fd7f045a5b9245044d0e852e7952d05?rik=zXFnCnYCLsvf1Q&pid=ImgRaw&r=0",
        ///           "https://th.bing.com/th/id/OIP.ruPnrBQCKcBxafwMiXdVjAHaHa?rs=1&pid=ImgDetMain",
        ///           "https://img.zcool.cn/community/0175f65cfa44dea801213ec215114a.jpg@2o.jpg"
        ///         ]
        ///     }
        /// </remarks>
        [HttpPut("UpdateProductImages")]
        public async Task<IActionResult> UpdateProductImages(UpdateProductImagesRequest request)
        {
            if (request.ImageUrlList.Count < 1) return BadRequest("請選擇上傳檔案");

            var result = await _productService.UpdateProductImagesAsync(request.ProductId, request.ImageUrlList);
            if (result.IsSuccess) return Ok(result.ResultDTO);
            return BadRequest(result.ErrorMessage);
        }


        [HttpPost("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts(GetAllProductsRequest request)
        {
            var result = await _productService.GetAllProductsAsync(request);
            if (result.IsSuccess) return Ok(result.ResultDTO);
            return BadRequest(result.ErrorMessage);
        }
    }
}
