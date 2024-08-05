using ApplicationCore.Models;

namespace ApplicationCore.DTOs.OrderDTOS
{
    public class CreateOrderResponse : BaseOperationResult
    {
        public int OrderId { get; set; }
    }
}