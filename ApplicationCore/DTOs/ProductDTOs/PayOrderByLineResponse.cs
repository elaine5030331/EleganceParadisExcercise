using ApplicationCore.Models;

namespace ApplicationCore.DTOs.ProductDTOs
{
    public class PayOrderByLineResponse : BaseOperationResult
    {
        public string WebPaymentURL { get; set; }
    }
}