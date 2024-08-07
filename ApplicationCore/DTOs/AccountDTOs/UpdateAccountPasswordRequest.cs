namespace ApplicationCore.DTOs.AccountDTOs
{
    public class UpdateAccountPasswordRequest
    {
        public int AccountId { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
