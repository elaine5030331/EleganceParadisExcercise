namespace ApplicationCore.DTOs.ProductDTOs
{
    public class ProductQueryResultDTO
    {
        public int ProductId { get; set; }
        public string CategoryName { get; set; }
        public string ProductName { get; set; }
        public string Spu { get; set; }
        public string Description { get; set; }
        public int SpecId { get; set; }
        public string Sku { get; set; }
        public decimal UnitPrice { get; set; }
        public string SpecName { get; set; }
        public int SpecOrder { get; set; }
        public int? StockQuantity { get; set; }
        public int ProductImageId { get; set; }
        public string ProductImageUrl { get; set; }
        public int ProductImageOrder { get; set; }

    }
    public class ProductDTO
    {
        public int ProductId { get; set; }
        public string CategoryName { get; set; }
        public string ProductName { get; set; }
        public string Spu { get; set; }
        public string Description { get; set; }
        public List<SpecDTO> Specs { get; set; }
        public List<ProductImageDTO> ProductImages { get; set; }
    }

    public class SpecDTO
    {
        public int SpecId { get; set; }
        public string Sku { get; set; }
        public decimal UnitPrice { get; set; }
        public string SpecName { get; set; }
        public int SpecOrder { get; set; }
        public int? StockQuantity { get; set; }
    }

    public class ProductImageDTO
    {
        public int ProductImageId { get; set; }
        public string ProductImageUrl { get; set; }
        public int ProductImageOrder { get; set; }
    }
}
