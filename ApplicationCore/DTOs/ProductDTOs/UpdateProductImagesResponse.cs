using ApplicationCore.Models;

namespace ApplicationCore.DTOs.ProductDTOs
{
    public class UpdateProductImagesResponse : BaseOperationResult
    {
        public int ProductId { get; set; }
        public List<string> ImageUrlList { get; set; }
    }
}