using ApplicationCore.Constants;
using ApplicationCore.DTOs.CategoryDTOs;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EleganceParadisAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// 取得所有商品類別
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Sample response:<br/>
        ///     {
        ///         [
        ///             {
        ///                 "id": 2,
        ///                 "name": "女性香氛",
        ///                 "order": -1,
        ///                 "imageURL": null,
        ///                 "description": "女性香氛",
        ///                 "subCategory": [
        ///                     {
        ///                         "id": 4,
        ///                         "name": "香水",
        ///                         "order": 2,
        ///                         "imageURL": null,
        ///                         "description": "女性香水",
        ///                         "subCategory": null
        ///                     },
        ///                     {
        ///                         "id": 6,
        ///                         "name": "隨身噴香霧",
        ///                         "order": 3,
        ///                         "imageURL": null,
        ///                         "description": "女性隨身噴香霧",
        ///                         "subCategory": [
        ///                             {
        ///                                 "id": 8,
        ///                                 "name": "沐浴和身體保養",
        ///                                 "order": 4,
        ///                                 "imageURL": null,
        ///                                 "description": "女性沐浴和身體保養",
        ///                                 "subCategory": null
        ///                             }
        ///                    ]
        ///                }
        ///            ]
        ///        },
        ///        {
        ///            "id": 1,
        ///            "name": "男性香氛",
        ///            "order": 0,
        ///            "imageURL": null,
        ///            "description": "男性香氛",
        ///            "subCategory": [
        ///                {
        ///                    "id": 3,
        ///                    "name": "香水",
        ///                    "order": 0,
        ///                    "imageURL": null,
        ///                    "description": "男性香水",
        ///                    "subCategory": null
        ///                },
        ///                {
        ///                    "id": 5,
        ///                    "name": "隨身噴香霧",
        ///                    "order": 1,
        ///                    "imageURL": null,
        ///                    "description": "男性隨身噴香霧",
        ///                    "subCategory": [
        ///                        {
        ///                            "id": 7,
        ///                            "name": "沐浴和身體保養",
        ///                            "order": 4,
        ///                            "imageURL": null,
        ///                            "description": "男性沐浴和身體保養",
        ///                            "subCategory": null
        ///                        }
        ///                    ]
        ///                }
        ///            ]
        ///        }
        ///    ]
        ///}
        /// </remarks>
        /// <response code ="200">取得所有商品類別成功</response>
        [HttpGet]
public async Task<IActionResult> GetAllCategory()
{
    var result = await _categoryService.GetCategories();
    return Ok(result);
}
    }
}
