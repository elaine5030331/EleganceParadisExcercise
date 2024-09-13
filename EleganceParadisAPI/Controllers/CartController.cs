using ApplicationCore.DTOs.CartDTO;
using ApplicationCore.Interfaces;
using EleganceParadisAPI.DTOs;
using EleganceParadisAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EleganceParadisAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        /// <summary>
        /// 新增商品至購物車
        /// </summary>
        /// <param name="addCartItemDTO"></param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:<br/>
        ///     {
        ///        "accountId": 1, 
        ///        "specId": 1(商品規格ID),
        ///        "quantity": 3(購物數量)"
        ///     }
        /// Sample response:<br/>
        ///      {
        ///       "accountId": 2,
        ///       "cartItems": [
        ///         {
        ///           "cartId": 2,
        ///           "selectedSpecId": 2,
        ///           "categoryName": "香水",
        ///           "productName": "木質調香水",
        ///           "productImage": "",
        ///           "specName": "100ml",
        ///           "unitPrice": 7777,
        ///           "quantity": 100,
        ///           "stock": null, //(null: 無限庫存)
        ///           "specs": [
        ///             {
        ///               "specId": 2,
        ///               "specName": "100ml",
        ///               "unitPrice": 7777,
        ///               "stock": null //(null: 無限庫存)
        ///             }
        ///           ]
        ///         },
        ///         {
        ///         "cartId": 5,
        ///           "selectedSpecId": 1,
        ///           "categoryName": "香水",
        ///           "productName": "木質調香水",
        ///           "productImage": "",
        ///           "specName": "50ml",
        ///           "unitPrice": 999,
        ///           "quantity": 200,
        ///           "stock": 4,
        ///           "specs": [
        ///             {
        ///             "specId": 1,
        ///               "specName": "50ml",
        ///               "unitPrice": 999,
        ///               "stock": 4
        ///             },
        ///             {
        ///             "specId": 3,
        ///               "specName": "100ml",
        ///               "unitPrice": 3999,
        ///               "stock": 5
        ///             }
        ///           ]
        ///         }
        ///       ],
        ///       "shippingFee": 130,
        ///       "subTotal": 977500,
        ///       "cartTotal": 977630,
        ///       "paymentTypes": [
        ///         {
        ///           "type": 0,
        ///           "displayName": "LinePay",
        ///           "icon": ""
        ///         }
        ///       ]
        ///     }
        /// </remarks>
        /// <response code ="200">新增商品至購物車成功</response>
        /// <response code ="400">
        ///     1.找不到這個用戶 <br/>
        ///     2.找不到AccountId對應的用戶<br/>
        ///     3.加入購物車失敗(僅會回傳購物車原本的資料)
        /// </response>
        [HttpPost("AddCartItem")]
        public async Task<IActionResult> AddCartItem(AddCartItemDTO addCartItemDTO)
        {
            var getAccountIdRes = User.GetAccountId();
            if (getAccountIdRes == null) return BadRequest("查無此人");
            if (getAccountIdRes != addCartItemDTO.AccountId) return BadRequest("AccountId不符");

            var result = await _cartService.AddCartItemAsync(addCartItemDTO);
            if (result.IsSuccess) return Ok(result.ResultDTO);

            return result.GetBadRequestResult();
        }

        /// <summary>
        /// 取得購物車內容
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        /// <response code ="200">取得購物車內容成功</response>
        /// <response code ="400">
        ///     1.找不到這個用戶 <br/>
        ///     2.找不到AccountId對應的用戶<br/>
        ///     3.加入購物車失敗(僅會回傳購物車原本的資料)
        /// </response>
        /// <remarks>
        /// Sample response:<br/>
        ///      {
        ///       "accountId": 2,
        ///       "cartItems": [
        ///         {
        ///           "cartId": 2,
        ///           "selectedSpecId": 2,
        ///           "categoryName": "香水",
        ///           "productName": "木質調香水",
        ///           "productImage": "",
        ///           "specName": "100ml",
        ///           "unitPrice": 7777,
        ///           "quantity": 100,
        ///           "stock": null, //(null: 無限庫存)
        ///           "specs": [
        ///             {
        ///               "specId": 2,
        ///               "specName": "100ml",
        ///               "unitPrice": 7777,
        ///               "stock": null //(null: 無限庫存)
        ///             }
        ///           ]
        ///         },
        ///         {
        ///         "cartId": 5,
        ///           "selectedSpecId": 1,
        ///           "categoryName": "香水",
        ///           "productName": "木質調香水",
        ///           "productImage": "",
        ///           "specName": "50ml",
        ///           "unitPrice": 999,
        ///           "quantity": 200,
        ///           "stock": 4,
        ///           "specs": [
        ///             {
        ///             "specId": 1,
        ///               "specName": "50ml",
        ///               "unitPrice": 999,
        ///               "stock": 4
        ///             },
        ///             {
        ///             "specId": 3,
        ///               "specName": "100ml",
        ///               "unitPrice": 3999,
        ///               "stock": 5
        ///             }
        ///           ]
        ///         }
        ///       ],
        ///       "shippingFee": 130,
        ///       "subTotal": 977500,
        ///       "cartTotal": 977630,
        ///       "paymentTypes": [
        ///         {
        ///           "type": 0,
        ///           "displayName": "LinePay",
        ///           "icon": ""
        ///         }
        ///       ]
        ///     }
        /// </remarks>
        [HttpGet("GetCartItems/{accountId}")]
        public async Task<IActionResult> GetCartItems(int accountId)
        {
            var getAccountIdRes = User.GetAccountId();
            if (getAccountIdRes == null) return BadRequest("查無此人");
            if (getAccountIdRes != accountId) return BadRequest("AccountId不符");

            var result = await _cartService.GetCartItemsAsync(accountId);
            if (result.IsSuccess) return Ok(result.ResultDTO);
            return result.GetBadRequestResult();
            //var errorResult = new BadRequestDTO
            //{
            //    ErrorMessage = result.ErrorMessage,
            //    Result = result.ResultDTO
            //};
            //return BadRequest(errorResult);
        }

        /// <summary>
        /// 更新購物車內容
        /// </summary>
        /// <param name="updateCartItemDTO"></param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:<br/>
        ///     {
        ///        "accountId": 1, 
        ///        "cartId": 1(購物車ID),
        ///        "specId": 1(商品規格ID),
        ///        "quantity": 3(購物數量)"
        ///     }
        /// Sample response:<br/>
        ///      {
        ///       "accountId": 2,
        ///       "cartItems": [
        ///         {
        ///           "cartId": 2,
        ///           "selectedSpecId": 2,
        ///           "categoryName": "香水",
        ///           "productName": "木質調香水",
        ///           "productImage": "",
        ///           "specName": "100ml",
        ///           "unitPrice": 7777,
        ///           "quantity": 100,
        ///           "stock": null, //(null: 無限庫存)
        ///           "specs": [
        ///             {
        ///               "specId": 2,
        ///               "specName": "100ml",
        ///               "unitPrice": 7777,
        ///               "stock": null //(null: 無限庫存)
        ///             }
        ///           ]
        ///         },
        ///         {
        ///         "cartId": 5,
        ///           "selectedSpecId": 1,
        ///           "categoryName": "香水",
        ///           "productName": "木質調香水",
        ///           "productImage": "",
        ///           "specName": "50ml",
        ///           "unitPrice": 999,
        ///           "quantity": 200,
        ///           "stock": 4,
        ///           "specs": [
        ///             {
        ///             "specId": 1,
        ///               "specName": "50ml",
        ///               "unitPrice": 999,
        ///               "stock": 4
        ///             },
        ///             {
        ///             "specId": 3,
        ///               "specName": "100ml",
        ///               "unitPrice": 3999,
        ///               "stock": 5
        ///             }
        ///           ]
        ///         }
        ///       ],
        ///       "shippingFee": 130,
        ///       "subTotal": 977500,
        ///       "cartTotal": 977630,
        ///       "paymentTypes": [
        ///         {
        ///           "type": 0,
        ///           "displayName": "LinePay",
        ///           "icon": ""
        ///         }
        ///       ]
        ///     }
        /// </remarks>
        /// <response code ="200">更新購物車內容成功</response>
        /// <response code ="400">
        ///     1.找不到這個用戶 <br/>
        ///     2.找不到AccountId對應的用戶<br/>
        ///     3.加入購物車失敗(僅會回傳購物車原本的資料)
        /// </response>
        [HttpPatch("UpdateCartItems")]
        public async Task<IActionResult> UpdateCartItems(UpdateCartItemDTO updateCartItemDTO)
        {
            var accountId = User.GetAccountId();
            if (accountId == null) return BadRequest("查無此人");
            if (accountId != updateCartItemDTO.AccountId) return BadRequest("AccountId 不符");

            var result = await _cartService.UpdateCartItemsAsync(updateCartItemDTO);
            if (result.IsSuccess) return Ok(result.ResultDTO);
            return result.GetBadRequestResult();
        }

        /// <summary>
        /// 刪除購物車資料
        /// </summary>
        /// <param name="deleteCartItemDTO"></param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:<br/>
        ///     {
        ///        "accountId": 1, 
        ///        "cartId": 1(購物車ID)
        ///     }
        /// Sample response:<br/>
        ///      {
        ///       "accountId": 2,
        ///       "cartItems": [
        ///         {
        ///           "cartId": 2,
        ///           "selectedSpecId": 2,
        ///           "categoryName": "香水",
        ///           "productName": "木質調香水",
        ///           "productImage": "",
        ///           "specName": "100ml",
        ///           "unitPrice": 7777,
        ///           "quantity": 100,
        ///           "stock": null, //(null: 無限庫存)
        ///           "specs": [
        ///             {
        ///               "specId": 2,
        ///               "specName": "100ml",
        ///               "unitPrice": 7777,
        ///               "stock": null //(null: 無限庫存)
        ///             }
        ///           ]
        ///         },
        ///         {
        ///         "cartId": 5,
        ///           "selectedSpecId": 1,
        ///           "categoryName": "香水",
        ///           "productName": "木質調香水",
        ///           "productImage": "",
        ///           "specName": "50ml",
        ///           "unitPrice": 999,
        ///           "quantity": 200,
        ///           "stock": 4,
        ///           "specs": [
        ///             {
        ///             "specId": 1,
        ///               "specName": "50ml",
        ///               "unitPrice": 999,
        ///               "stock": 4
        ///             },
        ///             {
        ///             "specId": 3,
        ///               "specName": "100ml",
        ///               "unitPrice": 3999,
        ///               "stock": 5
        ///             }
        ///           ]
        ///         }
        ///       ],
        ///       "shippingFee": 130,
        ///       "subTotal": 977500,
        ///       "cartTotal": 977630,
        ///       "paymentTypes": [
        ///         {
        ///           "type": 0,
        ///           "displayName": "LinePay",
        ///           "icon": ""
        ///         }
        ///       ]
        ///     }
        /// </remarks>
        /// <response code ="200">刪除購物車資料成功</response>
        /// <response code ="400">
        ///     1.找不到這個用戶 <br/>
        ///     2.找不到AccountId對應的用戶<br/>
        ///     3.加入購物車失敗(僅會回傳購物車原本的資料)
        /// </response>
        [HttpPost("DeleteCartItem")]
        public async Task<IActionResult> DeleteCartItem(DeleteCartItemDTO deleteCartItemDTO)
        {
            var accountId = User.GetAccountId();
            if (accountId == null) return BadRequest("查無此人");
            if (accountId != deleteCartItemDTO.AccountId) return BadRequest("AccountId 不符");

            var result = await _cartService.DeleteCartItemAsync(deleteCartItemDTO);
            if (result.IsSuccess) return Ok(result.ResultDTO);
            return result.GetBadRequestResult();
        }
    }
}
