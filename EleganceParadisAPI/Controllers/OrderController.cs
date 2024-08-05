using ApplicationCore.DTOs.OrderDTOS;
using ApplicationCore.Helpers;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EleganceParadisAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// 建立訂單
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder(CreateOrderRequest request)
        {
            if (string.IsNullOrEmpty(request.Purchaser)) return BadRequest("請填寫購買人姓名");

            if(!ValidateHelper.TryValidateMobile(request.PurchaserTel, out var mobileErrorMsg))
                return BadRequest(mobileErrorMsg);

            if(!ValidateHelper.TryValidateEmail(request.PurchaserEmail, out var emailErrorMsg))
                return BadRequest(emailErrorMsg);

            if (string.IsNullOrEmpty(request.City)) return BadRequest("請輸入縣市");
            if (string.IsNullOrEmpty(request.District)) return BadRequest("請輸入行政區");
            if (string.IsNullOrEmpty(request.Address)) return BadRequest("請輸入地址");

            var result = await _orderService.CreateOrder(request);
            if(result.IsSuccess) return Ok(result.ResultDTO);
            return BadRequest(result.ErrorMessage);
        }
    }
}
