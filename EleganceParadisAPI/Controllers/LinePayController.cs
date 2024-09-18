using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EleganceParadisAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LinePayController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public LinePayController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// 取得LinePay付款連結
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        /// <remarks>
        /// WebPaymentURL:Line pay回傳的URL
        /// </remarks>
        /// <response code ="200">取得LinePay付款連結成功</response>
        /// <response code ="400">
        /// 1. 查無對應的訂單資訊
        /// 2. 付款失敗
        /// </response>
        [HttpGet("PayOrder")]
        [Authorize]
        public async Task<IActionResult> PayOrder(int orderId)
        {
            var result = await _paymentService.PayOrderByLineAsync(orderId);
            if(result.IsSuccess) return Ok(result.ResultDTO);
            return BadRequest(result.ErrorMessage);
        }

        /// <summary>
        /// LinePay server callback
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpGet("ComfirmPayment")]
        public async Task<IActionResult> ComfirmPayment([FromQuery] string transactionId, [FromQuery] string orderId)
        {
            var result = await _paymentService.ConfirmPaymentAsync(transactionId, orderId);
            if (result.IsSuccess) return Redirect($"https://eleganceparadisapp.azurewebsites.net/cart/finish?orderId={orderId}");
            return BadRequest(result.ErrorMessage);
        }

        /// <summary>
        /// LinePay server callback
        /// </summary>
        /// <returns></returns>
        [HttpGet("CancelPayment")]
        public IActionResult CancelPayment()
        {
            return Redirect("https://eleganceparadisapp.azurewebsites.net/");
        }
    }
}
