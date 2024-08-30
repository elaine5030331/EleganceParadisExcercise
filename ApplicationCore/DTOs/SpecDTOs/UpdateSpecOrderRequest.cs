using ApplicationCore.DTOs.CategoryDTOs;

namespace ApplicationCore.DTOs.SpecDTOs
{
    public class UpdateSpecOrderRequest
    {
        public int ProductId { get; set; }
        public List<int> SpecIdList { get; set; }

    }
}