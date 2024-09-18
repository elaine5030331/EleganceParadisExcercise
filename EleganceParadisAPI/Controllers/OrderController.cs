using ApplicationCore.DTOs.OrderDTOS;
using ApplicationCore.Helpers;
using ApplicationCore.Interfaces;
using EleganceParadisAPI.Helpers;
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
        /// <remarks>
        /// Sample request:<br/>
        ///     {
        ///         "accountId": 0,
        ///         "purchaser": "ElaineKang",
        ///         "purchaserTel": "0960298440",
        ///         "purchaserEmail": "test@gmail.com",
        ///         "paymentType": LinePay = 0, ECPay = 1
        ///         "city": "台北市",
        ///         "district": "北投區",
        ///         "address": "實踐街1號"
        ///     }
        /// Sample response:<br/>
        ///     {
        ///         "orderId": 1
        ///     }
        /// </remarks>
        /// <response code ="200">建立訂單成功</response>
        /// <response code ="400">
        /// 1. 請填寫購買人姓名
        /// 2. 電話號碼格式有誤
        /// 3. 電子郵件格式有誤
        /// 4. 請輸入縣市
        /// 5. 請輸入行政區
        /// 6. 請輸入地址
        /// 7. 目前購物車沒有商品
        /// 8. 找不到對應的規格Id：{規格ID}
        /// 9. {產品名稱}庫存量不夠，庫存剩餘{規格庫存量}
        /// 10. 訂單成立失敗
        /// </response>
        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder(CreateOrderRequest request)
        {
            if (string.IsNullOrEmpty(request.Purchaser)) return BadRequest("請填寫購買人姓名");

            if (!ValidateHelper.TryValidateMobile(request.PurchaserTel, out var mobileErrorMsg))
                return BadRequest(mobileErrorMsg);

            if (!ValidateHelper.TryValidateEmail(request.PurchaserEmail, out var emailErrorMsg))
                return BadRequest(emailErrorMsg);

            if (string.IsNullOrEmpty(request.City)) return BadRequest("請輸入縣市");
            if (string.IsNullOrEmpty(request.District)) return BadRequest("請輸入行政區");
            if (string.IsNullOrEmpty(request.Address)) return BadRequest("請輸入地址");

            var result = await _orderService.CreateOrderAsync(request);
            if (result.IsSuccess) return Ok(result.ResultDTO);
            return BadRequest(result.ErrorMessage);
        }

        /// <summary>
        /// 取得單筆訂單
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:<br/>
        ///     {
        ///         "orderId": 1
        ///     }
        /// Sample response:<br/>
        ///     {
        ///         "orderId": 1,
        ///         "accountId": 2,
        ///         "orderNo": "EP20240806005528941"(訂單編號),
        ///         "purchaser": "test",
        ///         "purchaserEmail": "test@gmail.com",
        ///         "purchaserTel": "0988888888",
        ///         "paymentType": 0,(0 = LinePay, 1 = ECPay)
        ///         "orderStatus": 1,( Pending = 0, Paid = 1, Failed = 10)
        ///         "orderDate": "2024/08/06",
        ///         "createAt": 1722876928,(時間戳)
        ///         "address": "City + District + Address",
        ///         "orderDetails": [
        ///           {
        ///             "productName": "木質調香水_100ml",
        ///             "sku": "MPFWOOD_100",
        ///             "unitPrice": 7777,
        ///             "quantity": 3
        ///           },
        ///           {
        ///             "productName": "木質調香水_50ml",
        ///             "sku": "MPFWOOD_50",
        ///             "unitPrice": 999,
        ///             "quantity": 3
        ///           }
        ///         ]
        ///     }
        /// </remarks>
        /// <response code ="200">取得單筆訂單成功</response>
        /// <response code ="404">找不到此訂單</response>
        [HttpGet("GetOrder/{orderId}")]
        public async Task<IActionResult> GetOrder(int orderId)
        {
            var accountId = User.GetAccountId();
            var result = await _orderService.GetOrderAsync(orderId, accountId.Value);
            if (result == null) return NotFound();
            return Ok(result);
        }

        /// <summary>
        /// 取得訂單列表，無訂單內容會回傳空陣列
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:<br/>
        ///     {
        ///         "accountId": 2
        ///     } 
        /// Sample response:<br/>
        ///     [{
        ///         "orderId": 1,
        ///         "accountId": 2,
        ///         "orderNo": "EP20240806005528941"(訂單編號),
        ///         "purchaser": "test",
        ///         "purchaserEmail": "test@gmail.com",
        ///         "purchaserTel": "0988888888",
        ///         "paymentType": 0,(0 = LinePay, 1 = ECPay)
        ///         "orderStatus": 1,( Pending = 0, Paid = 1, Failed = 10)
        ///         "orderDate": "2024/08/06",
        ///         "createAt": 1722876928,(時間戳)
        ///         "address": "City + District + Address",
        ///         "orderDetails": [
        ///           {
        ///             "productName": "木質調香水_100ml",
        ///             "sku": "MPFWOOD_100",
        ///             "unitPrice": 7777,
        ///             "quantity": 3
        ///           },
        ///           {
        ///             "productName": "木質調香水_50ml",
        ///             "sku": "MPFWOOD_50",
        ///             "unitPrice": 999,
        ///             "quantity": 3
        ///           }
        ///         ]
        ///     }],
        /// </remarks>
        /// <response code ="200">取得訂單列表成功(無訂單內容會回傳空陣列)</response>
        [HttpGet("GetOrderList/{accountId}")]
        public async Task<IActionResult> GetOrderList(int accountId)
        {
            return Ok(await _orderService.GerOrderListAsync(accountId));
        }

        /// <summary>
        /// 透過訂單狀態取得對應的訂單內容
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:<br/>
        ///     {
        ///         "accountId": 2
        ///     }
        /// Sample response:<br/>
        ///     [{
        ///         "orderId": 1,
        ///         "accountId": 2,
        ///         "orderNo": "EP20240806005528941"(訂單編號),
        ///         "purchaser": "test",
        ///         "purchaserEmail": "test@gmail.com",
        ///         "purchaserTel": "0988888888",
        ///         "paymentType": 0,(0 = LinePay, 1 = ECPay)
        ///         "orderStatus": 1,( Pending = 0, Paid = 1, Failed = 10)
        ///         "orderDate": "2024/08/06",
        ///         "createAt": 1722876928,(時間戳)
        ///         "address": "City + District + Address",
        ///         "orderDetails": [
        ///           {
        ///             "productName": "木質調香水_100ml",
        ///             "sku": "MPFWOOD_100",
        ///             "unitPrice": 7777,
        ///             "quantity": 3
        ///           },
        ///           {
        ///             "productName": "木質調香水_50ml",
        ///             "sku": "MPFWOOD_50",
        ///             "unitPrice": 999,
        ///             "quantity": 3
        ///           }
        ///         ]
        ///     }],
        /// </remarks>
        /// <response code ="200">取得訂單內容成功</response>
        [HttpGet("GetOrderListByStatus/{accountId}")]
        public async Task<IActionResult> GetOrderListByStatus(int accountId)
        {
            var orders = await _orderService.GerOrderListAsync(accountId);

            var result = orders.GroupBy(o => o.OrderStatus).OrderBy(o => o.Key).ToList();

            return Ok(result);
        }
    }
}
