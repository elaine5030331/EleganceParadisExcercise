using Microsoft.AspNetCore.Http;

namespace EleganceParadisAPI.DTOs
{
    public class AddProductImagesRequest
    {
        public int ProductId { get; set; }
        public List<IFormFile> Files { get; set; }
    }
}