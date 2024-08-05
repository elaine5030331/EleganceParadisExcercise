using ApplicationCore.Enums;
namespace EleganceParadisAPI.DTOs.AuthDTOs
{
    public class GenerateTokenResponse
    {
        public int AccountId { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public long ExpireTime { get; set; }
        public AccountStatus AccountStatus { get; set; } 
    }
}
