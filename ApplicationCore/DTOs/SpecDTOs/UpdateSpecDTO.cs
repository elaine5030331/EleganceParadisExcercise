namespace ApplicationCore.DTOs.SpecDTOs
{
    public class UpdateSpecDTO
    {
        public int SpecId { get; set; }
        public string SKU { get; set; }
        public decimal UnitPrice { get; set; }
        public string SpecName { get; set; }
        public int Order { get; set; }
        public int? StockQuantity { get; set; }
    }
}
