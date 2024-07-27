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
        ///        "AccountId": 1, 
        ///        "SpecId": 1(商品規格ID),
        ///        "Quantity": 3(購物數量)"
        ///     }
        /// </remarks>
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
        /// <response code ="400">
        ///     1.找不到這個用戶 <br/>
        ///     2.找不到AccountId對應的用戶<br/>
        ///     3.加入購物車失敗(僅會回傳購物車原本的資料)
        /// </response>
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
        ///        "AccountId": 1, 
        ///        "SpecId": 1(商品規格ID),
        ///        "Quantity": 3(購物數量)"
        ///     }
        /// </remarks>
        /// <response code ="400">
        ///     1.找不到AccountId對應的用戶 <br/>
        ///     2.更新購物車失敗(僅會回傳購物車原本的資料)
        /// </response>
        [HttpPatch("UpdateCartItems")]
        public async Task<IActionResult> UpdateCartItems(UpdateCartItemDTO updateCartItemDTO)
        {
            var accountId = User.GetAccountId();
            if (accountId == null) return BadRequest("查無此人");
            if (accountId != updateCartItemDTO.AccountId) return BadRequest("AccountId 不符");

            var result = await _cartService.UpdateCartItemsAsync(updateCartItemDTO.AccountId, updateCartItemDTO);
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
        ///        "AccountId": 1, 
        ///        "CartId": 1(購物車ID)
        ///     }
        /// </remarks>
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
            if (accountId != deleteCartItemDTO.accountId) return BadRequest("AccountId 不符");

            var result = await _cartService.DeleteCartItemAsync(deleteCartItemDTO);
            if (result.IsSuccess) return Ok(result.ResultDTO);
            return result.GetBadRequestResult();
        }
    }
}
