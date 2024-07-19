using ApplicationCore.DTOs;
using ApplicationCore.DTOs.ProductDTOs;
using ApplicationCore.Interfaces;
using ApplicationCore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EleganceParadisAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpecController : ControllerBase
    {
        private readonly ISpecService _specService;

        public SpecController(ISpecService specService)
        {
            _specService = specService;
        }

        /// <summary>
        /// 新增產品規格
        /// </summary>
        /// <param name="specDTO"></param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:<br/>
        ///     {
        ///        "productId": 產品ID
        ///        "sku": 產品類別 + 產品名稱 + 產品規格
        ///        "unitPrice": 單價
        ///        "specName": 產品規格名稱(ex：50ml, 75ml, 1 peice)
        ///        "order": 排序
        ///        "stockQuantity": 庫存量
        ///     }
        /// </remarks>
        /// <response code ="201">產品規格新增成功</response>
        /// <response code ="400">產品規格新增失敗</response>
        [HttpPost("AddSpec")]
        public async Task<IActionResult> AddSpec(AddSpecDTO specDTO)
        {
            var result = await _specService.AddSpecAsync(specDTO);
            if (result.IsSuccess) return Created();
            return BadRequest(result.ErrorMessage);
        }

        /// <summary>
        /// 更新產品規格
        /// </summary>
        /// <param name="specId"></param>
        /// <param name="specDTO"></param>
        /// <returns></returns>
        /// <response code ="204">產品規格新增成功</response>
        /// <response code ="400">產品規格更新失敗</response>
        [HttpPut("UpdateSpec/{specId}")]
        public async Task<IActionResult> UpdateSpec(int specId, UpdateSpecDTO specDTO)
        {
            if (specId != specDTO.SpecId) return BadRequest();
            var result = await _specService.UpdateSpecAsync(specDTO);
            if(result.IsSuccess) return NoContent();
            return BadRequest(result.ErrorMessage);
        }
    }
}
