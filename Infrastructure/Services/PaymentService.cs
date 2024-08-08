using ApplicationCore.DTOs.ProductDTOs;
using ApplicationCore.Entities;
using ApplicationCore.Enums;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using LinePayApiSdk;
using LinePayApiSdk.DTOs;
using LinePayApiSdk.DTOs.Confirm;
using LinePayApiSdk.DTOs.Request;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IOrderService _orderService;
        private readonly LinePaySettings _linePaySettings;
        private readonly IRepository<Order> _orderRepo;
        private readonly IRepository<OrderDetail> _orderDetailRepo;
        private readonly LinePayApiOptions _linePayApiOptions;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(IOrderService orderService, LinePaySettings linePaySettings, IRepository<Order> orderRepo, IRepository<OrderDetail> orderDetailRepo, ILogger<PaymentService> logger)
        {
            _orderService = orderService;
            _linePaySettings = linePaySettings;
            _orderRepo = orderRepo;
            _orderDetailRepo = orderDetailRepo;
            _logger = logger;
            _linePayApiOptions = new LinePayApiOptions()
            {
                ChannelId = _linePaySettings.ChannelId,
                ChannelSecret = _linePaySettings.ChannelSecret,
                IsSandBox = _linePaySettings.IsSandBox,
                HttpClient = new HttpClient()
            };
        }

        public async Task<OperationResult<PayOrderByLineResponse>> PayOrderByLineAsync(int orderId)
        {
            //取出訂單資料
            var order = await _orderService.GerOrderAsync(orderId);
            if (order == null) return new OperationResult<PayOrderByLineResponse>("查無對應的訂單資訊");

            var total = order.OrderDetails.Sum(od => od.UnitPrice * od.Quantity);

            //敲RequestAPI
            var lineApi = new LinePayApi(_linePayApiOptions);

            var response = await lineApi.RequestAsync(new PaymentRequest
            {
                Currency = "TWD",
                OrderId = order.OrderNo,
                RedirectUrls = new RedirectUrls
                {
                    ConfirmUrl = _linePaySettings.ComfirmURL,
                    CancelUrl = _linePaySettings.CancelURL
                },
                Packages = new List<Package>
                {
                    new Package
                    {
                        Id = order.OrderId.ToString(),
                        Name = "EleganceParadis",
                        Products = new List<LinePayApiSdk.DTOs.Request.Product>
                        {
                            new LinePayApiSdk.DTOs.Request.Product
                            {
                                Name = "商品",
                                Quantity = 1,
                                Price = total
                            }
                        }
                    }
                }
            });

            //收到回傳的WebURL
            if (response == null) return new OperationResult<PayOrderByLineResponse>("付款失敗");
            if(response.ReturnCode != "0000") return new OperationResult<PayOrderByLineResponse>(response.ReturnMessage);

            return new OperationResult<PayOrderByLineResponse>()
            {
                IsSuccess = true,
                ResultDTO = new PayOrderByLineResponse
                {
                    WebPaymentURL = response.Info.PaymentUrl.Web
                }
            };

            //TODO
            //將transactionId存入資料庫
        }
        public async Task<OperationResult> ComfirmPaymentAsync(string transactionId, string orderNo)
        {
            try
            {
                //取得order資料，計算總金額
                var order = await _orderRepo.FirstOrDefaultAsync(o => o.OrderNo == orderNo);
                if (order == null) return new OperationResult("查無對應的訂單資料");
                var orderDetails = await _orderDetailRepo.ListAsync(od => od.OrderId == order.Id);
                var total = orderDetails.Sum(od => od.UnitPrice * od.Quantity);

                //敲ComfirmAPI
                var lineApi = new LinePayApi(_linePayApiOptions);
                var result = await lineApi.ConfirmAsync(transactionId, new ConfirmRequest
                {
                    Amount = total,
                    Currency = "TWD"
                });
                //判斷ReturnCode是否為成功，成功改變orderStaus
                if (result == null) return new OperationResult("comfirm失敗");
                if (result.ReturnCode != "0000") return new OperationResult(result.ReturnMessage);

                order.OrderStatus = OrderStatus.Paid;
                await _orderRepo.UpdateAsync(order);

                //TODO更新LinePay回傳資訊的資料表

                return new OperationResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new OperationResult("comfirm失敗");
            }
        }
    }
}
