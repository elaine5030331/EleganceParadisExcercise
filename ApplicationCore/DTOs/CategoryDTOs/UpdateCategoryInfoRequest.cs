namespace ApplicationCore.DTOs.CategoryDTOs
{
    public class UpdateCategoryInfoRequest
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string ImageURL { get; set; }
        public string Description { get; set; }
        public int? ParentCategoryId { get; set; }

    }
}