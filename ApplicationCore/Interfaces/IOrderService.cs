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
        public Task<OperationResult<CreateOrderResponse>> CreateOrder(CreateOrderRequest request);
    }
}
