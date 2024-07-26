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

        [HttpPost("AddCartItem")]
        public async Task<IActionResult> AddCartItem(AddCartItemDTO addCartItemDTO)
        {
            var getAccountIdRes = User.GetAccountId();
            if(getAccountIdRes == null) return BadRequest("查無此人");
            if (getAccountIdRes != addCartItemDTO.AccountId) return BadRequest("AccountId不符");

            var result = await _cartService.AddCartItemAsync(addCartItemDTO);
            if(result.IsSuccess) return Ok(result.ResultDTO);

            return result.GetBadRequestResult();
        }

        [HttpGet("GetCartItems/{accountId}")]
        public async Task<IActionResult> GetCartItems(int accountId)
        {
            var getAccountIdRes = User.GetAccountId();
            if (getAccountIdRes == null) return BadRequest("查無此人");
            if (getAccountIdRes != accountId) return BadRequest("AccountId不符");

            var result = await _cartService.GetCartItemsAsync(accountId);
            if(result.IsSuccess) return Ok(result.ResultDTO);
            return result.GetBadRequestResult();
            //var errorResult = new BadRequestDTO
            //{
            //    ErrorMessage = result.ErrorMessage,
            //    Result = result.ResultDTO
            //};
            //return BadRequest(errorResult);
        }
    }
}
