using ApplicationCore.Enums;

namespace ApplicationCore.DTOs.OrderDTOS
{
    public class OrderResponse
    {
        public int OrderId { get; set; }
        public int AccountId { get; set; }
        public string OrderNo { get; set; }
        public string Purchaser { get; set; }
        public string PurchaserEmail { get; set; }
        public string PurchaserTel { get; set; }
        public PaymentType PaymentType { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public string CreateTime { get; set; }
        public string Address { get; set; }
        public List<OrderDetailDTO>  OrderDetails { get; set; }
    }

    public class OrderDetailDTO
    {
        public string ProductName { get; set; }
        public string Sku { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}