namespace ApplicationCore.DTOs.CartDTO
{
    public class AddCartItemDTO
    {
        public int AccountId { get; set; }
        public int SpecId { get; set; }
        public int Quantity { get; set; }
    }
}
