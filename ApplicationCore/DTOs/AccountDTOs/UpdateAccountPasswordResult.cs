using ApplicationCore.Models;

namespace ApplicationCore.DTOs.AccountDTOs
{

    public class UpdateAccountPasswordResult : BaseOperationResult
    {
        public int AccountId { get; set; }
    }
}
