namespace ApplicationCore.DTOs.CartDTO
{
    public class UpdateCartItemDTO
    {
        public int AccountId { get; set; }
        public int CartId { get; set; }
        public int SpecId { get; set; }
        public int Quantity { get; set; }
    }
}