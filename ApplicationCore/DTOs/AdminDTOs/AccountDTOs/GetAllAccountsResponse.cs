using ApplicationCore.Models;

namespace ApplicationCore.DTOs.AdminDTOs.AccountDTOs
{
    public class GetAllAccountsResponse
    {
        public int AccountId { get; set; }
        public string Email { get; set; }
        public string AccountName { get; set; }
        public string Mobile { get; set; }
        public string CreateAt { get; set; }
        public int Status { get; set; }
    }
}