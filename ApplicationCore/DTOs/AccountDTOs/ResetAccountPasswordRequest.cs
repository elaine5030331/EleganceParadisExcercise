namespace ApplicationCore.DTOs.AccountDTOs
{
    public class ResetAccountPasswordRequest
    {
        public string EncodingParameter { get; set; }
        public string NewPassword { get; set; }
    }
}
