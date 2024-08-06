using ApplicationCore.DTOs.OrderDTOS;
using ApplicationCore.Entities;
using ApplicationCore.Enums;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderDetail> _orderDetailRepository;
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<Spec> _specRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly ILogger<OrderService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(ILogger<OrderService> logger, IUnitOfWork unitOfWork)
        {
            _orderRepository = unitOfWork.GetRepository<Order>();
            _orderDetailRepository = unitOfWork.GetRepository<OrderDetail>();
            _cartRepository = unitOfWork.GetRepository<Cart>();
            _specRepository = unitOfWork.GetRepository<Spec>();
            _productRepository = unitOfWork.GetRepository<Product>();
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<OperationResult<CreateOrderResponse>> CreateOrderAsync(CreateOrderRequest request)
        {
            await _unitOfWork.BeginAsync();
            try
            {
                //取得購物車內容
                var carts = await _cartRepository.ListAsync(c => c.AccountId == request.AccountId);
                if (carts.Count == 0)
                    return new OperationResult<CreateOrderResponse>("目前購物車沒有商品");

                var specs = await _specRepository.ListAsync(s => carts.Select(x => x.SpecId).Contains(s.Id));
                var products = await _productRepository.ListAsync(p => specs.Select(s => s.ProductId).Contains(p.Id));

                var errorMessage = new List<string>();
                var orderDetails = new List<OrderDetail>();
                var sequence = 1;
                //取得庫存、檢查庫存
                foreach (var cart in carts)
                {
                    var spec = specs.FirstOrDefault(s => s.Id == cart.SpecId);
                    if (spec == null) 
                        return new OperationResult<CreateOrderResponse>($"找不到對應的規格Id：{cart.SpecId}");

                    if(spec.StockQuantity == null) continue;

                    var productName = $"{products.FirstOrDefault(p => p.Id == spec.ProductId)?.ProductName ?? string.Empty}({spec.SpecName})" ;

                    if (cart.Quantity > spec.StockQuantity)
                    {
                        errorMessage.Add($"{productName}庫存量不夠，庫存剩餘{spec.StockQuantity}");
                        continue;
                    }

                    spec.StockQuantity = spec.StockQuantity - cart.Quantity;

                    orderDetails.Add(new OrderDetail
                    {
                        ProductName = productName,
                        Sku = spec.Sku,
                        UnitPrice = spec.UnitPrice,
                        Quantity = cart.Quantity,
                        Sequence = sequence,
                    });

                    sequence++;
                }

                //庫存量不夠→跳錯誤訊息
                if(errorMessage.Count > 0)
                {
                    var messages = string.Join(Environment.NewLine, errorMessage);
                    return new OperationResult<CreateOrderResponse>()
                    {
                        IsSuccess = false,
                        ErrorMessage = messages
                    };
                }
                await _specRepository.UpdateRangeAsync(specs);
                
                //庫存量夠→新增訂單

                var orderNo = $"EP{DateTime.Now.ToString("yyyyMMddHHmmssfff")}";

                var order = new Order
                {
                    AccountId = request.AccountId,
                    OrderNo = orderNo,
                    Purchaser = request.Purchaser,
                    PurchaserTel = request.PurchaserTel,
                    PurchaserEmail = request.PurchaserEmail,
                    OrderStatus = OrderStatus.Pending,
                    CreateAt = DateTimeOffset.UtcNow,
                    City = request.City,
                    District = request.District,
                    Address = request.Address,
                    PaymentType = request.PaymentType,
                    OrderDetails = orderDetails
                };

                await _orderRepository.AddAsync(order);

                //刪除購物車
                await _cartRepository.DeleteRangeAsync(carts);

                await _unitOfWork.CommitAsync();

                return new OperationResult<CreateOrderResponse>()
                {
                    IsSuccess = true,
                    ResultDTO = new CreateOrderResponse()
                    {
                        OrderId = order.Id
                    }
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, ex.Message);
                return new OperationResult<CreateOrderResponse>()
                {
                    IsSuccess = false,
                    ErrorMessage = "訂單成立失敗"
                };
            }
            finally
            {
                _unitOfWork.Dispose();
            }
           
        }

        public async Task<OrderResponse> GerOrderAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) return null;
            var orderDetails = await _orderDetailRepository.ListAsync(o => o.OrderId == orderId);
            if (orderDetails == null) return null;

            return GetOrderResponse(order, orderDetails);
        }

        private static OrderResponse GetOrderResponse(Order order, List<OrderDetail> orderDetails)
        {
            return new OrderResponse()
            {
                OrderId = order.Id,
                AccountId = order.AccountId,
                OrderNo = order.OrderNo,
                Purchaser = order.Purchaser,
                PurchaserEmail = order.PurchaserEmail,
                PurchaserTel = order.PurchaserTel,
                PaymentType = order.PaymentType,
                OrderStatus = order.OrderStatus,
                OredreDate = order.CreateAt.AddHours(8).ToString("yyyy/MM/dd"),
                CreateAt = order.CreateAt.ToUnixTimeSeconds(),
                Address = $"{order.City}{order.District}{order.Address}",
                OrderDetails = orderDetails.OrderBy(od => od.Sequence).Select(od => new OrderDetailDTO()
                {
                    ProductName = od.ProductName,
                    Sku = od.Sku,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice
                }).ToList()
            };
        }

        public async Task<List<OrderResponse>> GerOrderListAsync(int accountId)
        {
            var orders = (await _orderRepository.ListAsync(o => o.AccountId == accountId))
                                                .OrderByDescending(o => o.CreateAt)
                                                .ToList();
            if (orders.Count == 0) return new List<OrderResponse>();

            var allOrderDetails = await _orderDetailRepository.ListAsync(od => orders.Select(o => o.Id).Contains(od.OrderId));

            var result = new List<OrderResponse>();
            foreach(var order in orders)
            {
                var orderDetails = allOrderDetails.Where(od => od.OrderId == order.Id).ToList();

                result.Add(GetOrderResponse(order, orderDetails));
            }

            return result;
            
        }
    }
}
