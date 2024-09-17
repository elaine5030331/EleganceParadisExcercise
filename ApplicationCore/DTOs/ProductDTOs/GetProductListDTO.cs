namespace ApplicationCore.DTOs.ProductDTOs
{
    public class GetProductListDTO
    {
        public int CategoryId { get; set; }
        public int ProductId { get; set; }
        public string CategoryName { get; set; }
        public string ProductName { get; set; }
        //public decimal UnitPrice { get; set; }
        public string ProductImageUrl { get; set; }
        public List<SpecItem> SpecList { get; set; }

        public class SpecItem
        {
            public int SpecId { get; set; }
            public decimal UnitPrice { get; set; }
            public int? StockQuantity { get; set; }
        }
    }
}
