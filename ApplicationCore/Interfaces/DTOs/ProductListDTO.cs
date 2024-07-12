namespace ApplicationCore.Interfaces.DTOs
{
    public class ProductListDTO
    {
        public int ProductId { get; set; }
        public string CategoryName { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public string ProductImageUrl { get; set; }
    }
}
