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
            var result = await _paymentService.ComfirmPaymentAsync(transactionId, orderId);
            if (result.IsSuccess) return Redirect("https://eleganceparadis.azurewebsites.net/");
            return BadRequest(result.ErrorMessage);
        }

        /// <summary>
        /// LinePay server callback
        /// </summary>
        /// <returns></returns>
        [HttpGet("CancelPayment")]
        public IActionResult CancelPayment()
        {
            return Redirect("https://eleganceparadis.azurewebsites.net/");
        }
    }
}
