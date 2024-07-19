namespace ApplicationCore.DTOs.ProductDTOs
{
    public class GetProductListDTO
    {
        public int ProductId { get; set; }
        public string CategoryName { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public string ProductImageUrl { get; set; }
    }
}
