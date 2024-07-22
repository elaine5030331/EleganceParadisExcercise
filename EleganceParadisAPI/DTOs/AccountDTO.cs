using ApplicationCore.Models;

namespace EleganceParadisAPI.DTOs
{
    public class RegistDTO
    {
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmedPassword { get; set; }
    }

    public class CreateAccountResultDTO : BaseOperationResult
    {
        public int AccountId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
    }

    public class GetAccountInfoDTO
    {
        public int AccountId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
    }

    public class UpdateAccountInfo
    {
        public int AccountId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
    }

    public class UpdateAcoountInfoResult : BaseOperationResult
    {
        public int AccountId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
    }

    public class UpdateAccountPassword
    {
        public int AccountId { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class UpdateAccountPasswordResult : BaseOperationResult
    {
        public int AccountId { get; set; }
    }
}
