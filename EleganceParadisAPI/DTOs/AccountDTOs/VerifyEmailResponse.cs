using ApplicationCore.Models;

namespace EleganceParadisAPI.DTOs.AccountDTOs
{
    public class VerifyEmailResponse : BaseOperationResult
    {
        public int AccountId { get; set; }
        public string Email { get; set; }
    }
}
