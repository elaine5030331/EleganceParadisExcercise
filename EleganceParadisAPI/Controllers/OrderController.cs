using ApplicationCore.DTOs.OrderDTOS;
using ApplicationCore.Helpers;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EleganceParadisAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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

            var result = await _orderService.CreateOrderAsync(request);
            if(result.IsSuccess) return Ok(result.ResultDTO);
            return BadRequest(result.ErrorMessage);
        }

        /// <summary>
        /// 取得單筆訂單
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        /// <response code ="404">找不到此訂單</response>
        [HttpGet("GetOrder/{orderId}")]
        public async Task<IActionResult> GetOrder(int orderId)
        {
            var result = await _orderService.GerOrderAsync(orderId);
            if(result == null) return NotFound();
            return Ok(result);
        }

        /// <summary>
        /// 取得訂單列表，無訂單內容會回傳空陣列
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [HttpGet("GetOrderList/{accountId}")]
        public async Task<IActionResult> GetOrderList(int accountId)
        {
            return Ok(await _orderService.GerOrderListAsync(accountId));
        }

        [HttpGet("GetOrderListByStatus/{accountId}")]
        public async Task<IActionResult> GetOrderListByStatus(int accountId)
        {
            var orders = await _orderService.GerOrderListAsync(accountId);

            var result = orders.GroupBy(o => o.OrderStatus).OrderBy(o => o.Key).ToList();

            return Ok(result);
        }
    }
}
