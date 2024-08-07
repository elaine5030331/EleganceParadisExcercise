using ApplicationCore.Models;

namespace ApplicationCore.DTOs.AccountDTOs
{
    public class UpdateAcoountInfoResult : BaseOperationResult
    {
        public int AccountId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
    }
}
