using ApplicationCore.Constants;
using ApplicationCore.DTOs.CategoryDTOs;
using ApplicationCore.DTOs.ProductDTOs;
using ApplicationCore.Interfaces;
using EleganceParadisAPI.DTOs;
using EleganceParadisAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EleganceParadisAPI.AdminControllers
{
    [Route("api/AdminProduct")]
    [ApiController]
    [Authorize(Roles = EleganceParadisRole.Admin)]
    public class AdminProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IUploadImageService _imageService;

        public AdminProductController(IProductService productService, IUploadImageService imageService)
        {
            _productService = productService;
            _imageService = imageService;
        }

        /// <summary>
        /// 新增商品
        /// </summary>
        /// <param name="addProductDTO"></param>
        /// <returns></returns>
        /// <remarks>
        ///Sample request:<br/>
        ///     {
        ///        "categoryId": 類別ID(ex：香水、隨身噴香霧、沐浴與身體保養),
        ///        "spu": "商品編號",
        ///        "productName": "商品名稱",
        ///        "description": "商品描述",
        ///        "productImageList": ["string"]
        ///     }
        /// </remarks>
        /// <response code ="200">新增商品成功</response>
        /// <response code ="400">新增商品失敗</response>
        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct(AddProductDTO addProductDTO)
        {
            var result = await _productService.AddProductAsync(addProductDTO);
            if (result.IsSuccess) return Ok(result.ResultDTO);
            return BadRequest(result.ErrorMessage);
        }

        /// <summary>
        /// 更新商品資料
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="updateProductDTO"></param>
        /// <returns></returns>
        /// <response code ="204">更新商品資料成功</response>
        /// <response code ="400">
        /// 1. 參數異常
        /// 2. 找不到對應的商品
        /// 3. 更新商品資料失敗
        /// </response>
        [HttpPut("UpdateProduct/{productId}")]
        public async Task<IActionResult> UpdateProduct(int productId, UpdateProductDTO updateProductDTO)
        {
            if (productId != updateProductDTO.ProductId) return BadRequest("參數異常");
            var result = await _productService.UpdateProductAsync(productId, updateProductDTO);
            if (result.IsSuccess) return NoContent();
            return BadRequest(result.ErrorMessage);
        }

        /// <summary>
        /// 刪除商品
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        /// <response code ="204">刪除商品成功</response>
        /// <response code ="400">
        /// 1. 找不到對應的商品
        /// 2. 刪除商品失敗
        /// </response>
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
        /// <response code ="200">新增商品圖片成功</response>
        /// <response code ="400">
        /// 1. 檔案上傳格式有誤
        /// 2. 圖片上傳API異常
        /// 3. 商品圖片新增失敗
        /// </response>
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
        /// <response code ="200">更新商品圖片成功</response>
        /// <response code ="400">
        /// 1. 請選擇上傳檔案
        /// 2. 找不到對應的商品圖
        /// 3. 商品圖片更新失敗
        /// </response>
        [HttpPut("UpdateProductImages")]
        public async Task<IActionResult> UpdateProductImages(UpdateProductImagesRequest request)
        {
            if (request.ImageUrlList.Count < 1) return BadRequest("請選擇上傳檔案");

            var result = await _productService.UpdateProductImagesAsync(request.ProductId, request.ImageUrlList);
            if (result.IsSuccess) return Ok(result.ResultDTO);
            return BadRequest(result.ErrorMessage);
        }


        /// <summary>
        /// 取得所有商品清單
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code ="200">取得所有商品清單成功</response>
        /// <response code ="400">
        /// 1. 目前尚未有商品
        /// 2. 取得商品清單失敗
        /// </response>
        [HttpPost("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts(GetAllProductsRequest request)
        {
            var result = await _productService.GetAllProductsAsync(request);
            if (result.IsSuccess) return Ok(result.ResultDTO);
            return BadRequest(result.ErrorMessage);
        }
    }
}
