﻿using ApplicationCore.DTOs.ProductDTOs;
using ApplicationCore.Entities;
using ApplicationCore.Enums;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Settings;
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
            var order = await _orderService.GetOrderAsync(orderId);
            if (order == null) return new OperationResult<PayOrderByLineResponse>("查無對應的訂單資訊");

            var total = order.TotalAmount;

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
            //將 transactionId 存入資料庫
        }
        public async Task<OperationResult> ConfirmPaymentAsync(string transactionId, string orderNo)
        {
            try
            {
                //取得 order 資料，計算總金額
                var orderEntity = await _orderRepo.FirstOrDefaultAsync(o => o.OrderNo == orderNo);
                var order = await _orderService.GetOrderAsync(orderEntity.Id);
                if (orderEntity == null) return new OperationResult("查無對應的訂單資料");
                var total = order.TotalAmount;

                //敲ConfirmAPI
                var lineApi = new LinePayApi(_linePayApiOptions);
                var result = await lineApi.ConfirmAsync(transactionId, new ConfirmRequest
                {
                    Amount = total,
                    Currency = "TWD"
                });
                //判斷ReturnCode是否為成功，成功改變 orderStatus
                if (result == null) return new OperationResult("confirm失敗");
                if (result.ReturnCode != "0000") return new OperationResult(result.ReturnMessage);

                orderEntity.OrderStatus = OrderStatus.Paid;
                await _orderRepo.UpdateAsync(orderEntity);

                //TODO更新LinePay回傳資訊的資料表

                return new OperationResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new OperationResult("confirm失敗");
            }
        }
    }
}
