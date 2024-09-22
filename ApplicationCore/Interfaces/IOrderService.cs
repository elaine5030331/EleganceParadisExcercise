using ApplicationCore.DTOs.OrderDTOS;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IOrderService
    {
        Task<OperationResult<CreateOrderResponse>> CreateOrderAsync(CreateOrderRequest request);
        Task<List<OrderResponse>> GerOrderListAsync(int accountId);
        Task<OrderResponse> GetOrderAsync(int orderId);
        Task<OrderResponse> GetOrderAsync(string orderNo, int accountId);
    }
}
