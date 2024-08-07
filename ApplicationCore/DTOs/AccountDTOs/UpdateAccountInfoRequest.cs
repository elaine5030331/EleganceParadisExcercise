namespace ApplicationCore.DTOs.AccountDTOs;

public class UpdateAccountInfoRequest
{
    public int AccountId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Mobile { get; set; }
}
