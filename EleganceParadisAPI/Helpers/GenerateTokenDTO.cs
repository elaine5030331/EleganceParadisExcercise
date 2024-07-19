namespace EleganceParadisAPI.Helpers
{
    public class GenerateTokenDTO
    {
        public int AccountId { get; set; }
        public string Email { get; set; }
        //public IEnumerable<string> Roles { get; set; }
        public int ExpireMinutes { get; set; } = 60 * 12;
    }
}
