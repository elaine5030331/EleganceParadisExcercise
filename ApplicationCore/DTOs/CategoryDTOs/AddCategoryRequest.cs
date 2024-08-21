namespace ApplicationCore.DTOs.CategoryDTOs
{
    public class AddCategoryRequest
    {
        public string Name { get; set; }
        public string ImageURL { get; set; }
        public string Description { get; set; }
        public int? ParentCategoryId { get; set; }
    }
}