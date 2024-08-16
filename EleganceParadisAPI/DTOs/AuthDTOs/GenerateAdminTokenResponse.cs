namespace EleganceParadisAPI.DTOs.AuthDTOs
{
    public class GenerateAdminTokenResponse
    {
        public string AccessToken { get; set; }
        public long ExpireTime { get; set; }
    }
}