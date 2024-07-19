namespace ApplicationCore.DTOs
{
    public class AddSpecDTO
    {
        public int ProductId { get; set; }
        public string SKU { get; set; }
        public decimal UnitPrice { get; set; }
        public string SpecName { get; set; }
        public int StockQuantity { get; set; }
    }
}
