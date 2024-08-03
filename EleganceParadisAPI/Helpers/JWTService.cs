using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EleganceParadisAPI.Helpers
{
    public class JWTService
    {
        private readonly IConfiguration _configuration;
        private readonly IRepository<AuthTokenHistory> _repository;

        public JWTService(IConfiguration configuration, IRepository<AuthTokenHistory> repository)
        {
            _configuration = configuration;
            _repository = repository;
        }

        public async Task<GenerateTokenResponse> GenerateToken(GenerateTokenDTO generateTokenDTO)
        {
            var issuer = _configuration.GetValue<string>("JwtSettings:Issuer");
            var signKey = _configuration.GetValue<string>("JwtSettings:SignKey");

            //將所需資訊(使用者相關的資料)加入Claim(聲明)中
            var claims = new List<Claim>();
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, generateTokenDTO.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(ClaimTypes.PrimarySid, generateTokenDTO.AccountId.ToString()));

            ////新增角色
            //if (generateTokenDTO.Roles != null)
            //{
            //    foreach (var role in generateTokenDTO.Roles)
            //    {
            //        claims.Add(new Claim(ClaimTypes.Role, role));

            //    }
            //}

            //宣告身分識別參數
            var userClaimsIdentity = new ClaimsIdentity(claims);
            //建立對稱式加密金鑰(for JWT 簽章)
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signKey));
            //產生數位簽章的密碼編譯演算法
            var signingCretentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Issuer = issuer,
                Subject = userClaimsIdentity,
                Expires = DateTime.UtcNow.AddMinutes(generateTokenDTO.AccessTokenExpireMinutes),
                SigningCredentials = signingCretentials
            };

            //產生JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var serializeToken = tokenHandler.WriteToken(securityToken);

            //RefreshToken
            var refreshToken = Guid.NewGuid().ToString("N");

            //存進資料庫
            var expireTime = DateTimeOffset.UtcNow.AddDays(7);
            var entity = new AuthTokenHistory
            {
                AccountId = generateTokenDTO.AccountId,
                AccessToken = serializeToken,
                RefreshToken = refreshToken,
                ExpiredTime = expireTime,
                CreatAt = DateTimeOffset.UtcNow
            };

            await _repository.AddAsync(entity);

            return new GenerateTokenResponse()
            {
                AccountId = generateTokenDTO.AccountId,
                AccessToken = serializeToken,
                RefreshToken = refreshToken,
                ExpireTime = expireTime.ToUnixTimeSeconds()
            };

        }
    }

    public class GenerateTokenResponse
    {
        public int AccountId { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public long ExpireTime { get; set; }
    }
}
