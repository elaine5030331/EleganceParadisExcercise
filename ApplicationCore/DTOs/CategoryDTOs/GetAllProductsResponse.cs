using ApplicationCore.Models;

namespace ApplicationCore.DTOs.CategoryDTOs
{
    public class GetAllProductsResponse : BaseOperationResult
    {
        public List<ProductItem>  ProductList { get; set; }
        
    }

    public class ProductItem
    {
        public int CategoryId { get; set; }

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string SPU { get; set; }
        public bool Enable { get; set; }
        public string Description { get; set; }
        public string CreateAt { get; set; }
        public List<SpecItems> SpecList { get; set; }
        public List<Images> ImageList { get; set; }
    }

    public class Images
    {
        public int ProductImageId { get; set; }
        public string URL { get; set; }
    }

    public class SpecItems
    {
        public int SpecId { get; set; }
        public string SKU { get; set; }
        public decimal UnitPrice { get; set; }
        public string SpecName { get; set; }
        public int? StockQuantity { get; set; }
    }
}