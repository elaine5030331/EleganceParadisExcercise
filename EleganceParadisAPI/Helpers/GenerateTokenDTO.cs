namespace EleganceParadisAPI.Helpers
{
    public class GenerateTokenDTO
    {
        public int AccountId { get; set; }
        public string Email { get; set; }
        public int AccessTokenExpireMinutes { get; set; } = 15;
    }
}
