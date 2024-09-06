namespace ApplicationCore.DTOs.CategoryDTOs
{
    public class UpdateCategoryOrderRequest
    {
        public int? ParentCategoryId { get; set; }
        public List<int> SubCategoryIdList { get; set; }
    }
}