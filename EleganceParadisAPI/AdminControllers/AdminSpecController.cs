using ApplicationCore.Constants;
using ApplicationCore.DTOs.SpecDTOs;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EleganceParadisAPI.AdminControllers
{
    [Route("api/AdminSpec")]
    [ApiController]
    [Authorize(Roles = EleganceParadisRole.Admin)]
    public class AdminSpecController : ControllerBase
    {
        private readonly ISpecService _specService;

        public AdminSpecController(ISpecService specService)
        {
            _specService = specService;
        }

        /// <summary>
        /// 新增商品規格
        /// </summary>
        /// <param name="specDTO"></param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:<br/>
        ///     {
        ///        "productId": 商品ID
        ///        "sku": 商品類別 + 商品名稱 + 商品規格
        ///        "unitPrice": 單價
        ///        "specName": 商品規格名稱(ex：50ml, 75ml, 1 piece)
        ///        "stockQuantity": 庫存量
        ///     }
        /// </remarks>
        /// <response code ="201">商品規格新增成功</response>
        /// <response code ="400">商品規格新增失敗</response>
        [HttpPost("AddSpec")]
        public async Task<IActionResult> AddSpec(AddSpecDTO specDTO)
        {
            var result = await _specService.AddSpecAsync(specDTO);
            if (result.IsSuccess) return Created();
            return BadRequest(result.ErrorMessage);
        }

        /// <summary>
        /// 更新商品規格
        /// </summary>
        /// <param name="specId"></param>
        /// <param name="specDTO"></param>
        /// <returns></returns>
        /// <response code ="204">商品規格更新成功</response>
        /// <response code ="400">
        /// 1. 參數異常
        /// 2. 找不到對應的商品規格
        /// 2. 商品規格更新失敗
        /// </response>
        [HttpPut("UpdateSpec/{specId}")]
        public async Task<IActionResult> UpdateSpec(int specId, UpdateSpecDTO specDTO)
        {
            if (specId != specDTO.SpecId) return BadRequest("參數異常");
            var result = await _specService.UpdateSpecAsync(specDTO);
            if (result.IsSuccess) return NoContent();
            return BadRequest(result.ErrorMessage);
        }

        /// <summary>
        /// 刪除商品規格
        /// </summary>
        /// <param name="specId"></param>
        /// <returns></returns>
        /// <response code ="204">商品規格刪除成功</response>
        /// <response code ="400">
        /// 1. 找不到對應的商品規格
        /// 2. 商品規格刪除失敗
        /// </response>
        [HttpDelete("DeleteSpec/{specId}")]
        public async Task<IActionResult> DeleteSpec(int specId)
        {
            var result = await _specService.DeleteSpecAsync(specId);
            if (result.IsSuccess) return NoContent();
            return BadRequest(result.ErrorMessage);
        }

        /// <summary>
        /// 更新商品規格順序
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// /// <response code ="200">商品規格順序更新成功</response>
        /// <response code ="400">
        /// 1. ProductId無法找到對應的商品
        /// 2. 目前尚未建立對應的商品規格
        /// 3. 參數異常
        /// 4. 商品順序更新失敗
        /// </response>
        [HttpPut("UpdateSpecOrder")]
        public async Task<IActionResult> UpdateSpecOrder(UpdateSpecOrderRequest request)
        {
            var result = await _specService.UpdateSpecOrderAsync(request);
            if (result.IsSuccess) return Ok();
            return BadRequest(result.ErrorMessage);
        }
    }
}
