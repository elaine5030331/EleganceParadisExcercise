using ApplicationCore.Entities;
using ApplicationCore.Enums;
using ApplicationCore.Interfaces;
using EleganceParadisAPI.DTOs.AuthDTOs;
using EleganceParadisAPI.Helpers;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EleganceParadisAPI.Services
{
    public class JWTService
    {
        private readonly IConfiguration _configuration;
        private readonly IRepository<AuthTokenHistory> _authTokenRepo;
        private readonly IRepository<Account> _accountRepo;

        public JWTService(IConfiguration configuration, IRepository<AuthTokenHistory> authTokenRepo, IRepository<Account> accountRepo)
        {
            _configuration = configuration;
            _authTokenRepo = authTokenRepo;
            _accountRepo = accountRepo;
        }

        public async Task<GenerateTokenResponse> GenerateToken(GenerateTokenDTO generateTokenDTO)
        {
            var account = await _accountRepo.GetByIdAsync(generateTokenDTO.AccountId);
            if (account == null || account.Status != AccountStatus.Verified)
                return new GenerateTokenResponse()
                {
                    AccountId = generateTokenDTO.AccountId,
                    AccountStatus = account.Status,
                };

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

            var serializeToken = GenerateJWT(claims);

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

            await _authTokenRepo.AddAsync(entity);

            return new GenerateTokenResponse()
            {
                AccountId = generateTokenDTO.AccountId,
                AccessToken = serializeToken,
                RefreshToken = refreshToken,
                ExpireTime = expireTime.ToUnixTimeSeconds(),
                AccountStatus = account.Status
            };
        }

        public GenerateAdminTokenResponse GenerateAdminToken(string accountName, string roleName)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(JwtRegisteredClaimNames.Name, accountName));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(ClaimTypes.Role, roleName));

            var serializeToken = GenerateJWT(claims);

            return new GenerateAdminTokenResponse()
            {
                AccessToken = serializeToken,
                ExpireTime = DateTimeOffset.UtcNow.AddMinutes(60 * 24 * 7).ToUnixTimeSeconds(),
            };
        }

        private string GenerateJWT(List<Claim> claims)
        {
            var issuer = _configuration.GetValue<string>("JwtSettings:Issuer");
            var signKey = _configuration.GetValue<string>("JwtSettings:SignKey");

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
                Expires = DateTime.UtcNow.AddMinutes(60*7),
                SigningCredentials = signingCretentials
            };

            //產生JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var serializeToken = tokenHandler.WriteToken(securityToken);

            return serializeToken;
        }

        public async Task LogoutAsync(LogoutRequest request)
        {
            var entity = await _authTokenRepo.FirstOrDefaultAsync(x => x.RefreshToken == request.RefreshToken);
            if (entity == null) return;
            entity.ExpiredTime = DateTimeOffset.UtcNow;
            await _authTokenRepo.UpdateAsync(entity);
        }

        public async Task<GenerateTokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var entity = await _authTokenRepo.FirstOrDefaultAsync(x => x.RefreshToken == request.RefreshToken && x.AccessToken == request.AccessToken);

            if (entity == null) 
                throw new ArgumentException("參數異常");

            if (entity.ExpiredTime.CompareTo(DateTimeOffset.UtcNow) < 0)
                throw new InvalidOperationException("RefreshToken失效");

            var account = await _accountRepo.GetByIdAsync(entity.AccountId);

            return await GenerateToken(new GenerateTokenDTO
            {
                AccountId = entity.AccountId,
                Email = account.Email
            });
        }
    }
}
