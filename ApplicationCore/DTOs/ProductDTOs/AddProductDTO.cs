namespace ApplicationCore.DTOs.ProductDTOs
{
    public class AddProductDTO
    {
        public int CategoryId { get; set; }
        public string SPU { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
    }
}
