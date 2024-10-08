﻿using ApplicationCore.Interfaces;
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
        /// <remarks>
        ///Sample response:<br/>
        ///     {
        ///         [
        ///             {
        ///               "categoryId": 3,
        ///               "productId": 1,
        ///               "categoryName": "香水",
        ///               "productName": "木質調香水",
        ///               "productImageUrl": "https://eleganceparadisapp.azurewebsites.net/images/item_1.webp",
        ///               "specList": [
        ///                 {
        ///                   "specId": 1,
        ///                   "unitPrice": 999,
        ///                   "stockQuantity": 25
        ///                 },
        ///                 {
        ///                   "specId": 3,
        ///                   "unitPrice": 3999,
        ///                   "stockQuantity": 1
        ///                 }
        ///               ]
        ///             },
        ///         ]
        ///     }
        /// </remarks>
        /// <response code ="200">取得商品列表資訊成功</response>
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
        /// <remarks>
        ///Sample response:<br/>
        ///     {
        ///         "categoryId": 3,
        ///         "productId": 6,
        ///         "categoryName": "香水",
        ///         "productName": "豬",
        ///         "spu": "piggy",
        ///         "description": "string",
        ///         "specs": [
        ///           {
        ///             "specId": 5,
        ///             "sku": "",
        ///             "unitPrice": 0,
        ///             "specName": "",
        ///             "specOrder": 0,
        ///             "stockQuantity": null
        ///           }
        ///         ],
        ///         "productImages": [
        ///           {
        ///             "productImageId": 13,
        ///             "productImageUrl": "https://res.cloudinary.com/dupxtirfd/image/upload/v1723474937/EleganceParadis/tpj9cnfpchssqyf0wjc4.jpg",
        ///             "productImageOrder": 0
        ///           }
        ///         ]
        ///     }
        /// </remarks>
        /// <response code ="200">取得商品資料By productId成功</response>
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
