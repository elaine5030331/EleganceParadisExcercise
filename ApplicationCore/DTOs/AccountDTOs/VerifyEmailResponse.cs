using ApplicationCore.Models;

namespace ApplicationCore.DTOs.AccountDTOs
{
    public class VerifyEmailResponse : BaseOperationResult
    {
        public int AccountId { get; set; }
        public string Email { get; set; }
    }
}
