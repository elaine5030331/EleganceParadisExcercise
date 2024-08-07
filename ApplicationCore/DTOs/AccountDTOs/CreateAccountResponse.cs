using ApplicationCore.Models;

namespace ApplicationCore.DTOs.AccountDTOs
{
    public class CreateAccountResponse : BaseOperationResult
    {
        public int AccountId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
    }
}
