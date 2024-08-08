using ApplicationCore.DTOs.OrderDTOS;

namespace ApplicationCore.Helpers
{
    public class CreateOrderEmailRequest
    {
        public string OrderNo { get; set; }
        public string Purchaser { get; set; }
        public string OredreDate { get; set; }
        public decimal SumSubTotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderDetailDTO> OrderDetails { get; set; }
    }
}