namespace EleganceParadisAPI.DTOs
{
    public class UpdateProductImagesRequest
    {
        public int ProductId { get; set; }
        public List<string> ImageUrlList { get; set; }
    }
}
