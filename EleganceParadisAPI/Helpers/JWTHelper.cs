using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EleganceParadisAPI.Helpers
{
    public class JWTHelper
    {
        private readonly IConfiguration _configuration;

        public JWTHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public JWTDTO GenerateToken(string userName, IEnumerable<string> roles, int expireMinutes = 60)
        {
            var issuer = _configuration.GetValue<string>("JwtSettings:Issuer");
            var signKey = _configuration.GetValue<string>("JwtSettings:SignKey");

            //將所需資訊(使用者相關的資料)加入Claim(聲明)中
            var claims = new List<Claim>();
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, userName));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            //新增角色
            if(roles != null)
            {
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));

                }
            }

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
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
                SigningCredentials = signingCretentials
            };

            //產生JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var serializeToken = tokenHandler.WriteToken(securityToken);

            return new JWTDTO()
            {
                Token = serializeToken,
                ExpireTime = new DateTimeOffset(tokenDescriptor.Expires.Value).ToUnixTimeSeconds()
            };

        }
    }
    public class JWTDTO
    {
        public string Token { get; set; }
        public long ExpireTime { get; set; }
    }
}
