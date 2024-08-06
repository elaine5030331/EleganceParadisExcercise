using ApplicationCore.Enums;

namespace ApplicationCore.DTOs.OrderDTOS
{
    public class CreateOrderRequest
    {
        public int AccountId { get; set; }
        public string Purchaser { get; set; }
        public string PurchaserTel { get; set; }
        public string PurchaserEmail { get; set; }
        public PaymentType PaymentType { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Address { get; set; }
    }
}