using ApplicationCore.Interfaces;
using ApplicationCore.Settings;
using EleganceParadisAPI.DTOs.AuthDTOs;
using EleganceParadisAPI.Helpers;
using EleganceParadisAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EleganceParadisAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JWTService _jwtService;
        private readonly IUserManageService _userManageService;
        private readonly ILogger<AuthController> _logger;
        private readonly AdminInfoSettings _adminInfoSettings;

        public AuthController(JWTService jwtService, IUserManageService userManageService, ILogger<AuthController> logger, AdminInfoSettings adminInfoSettings)
        {
            _jwtService = jwtService;
            _userManageService = userManageService;
            _logger = logger;
            _adminInfoSettings = adminInfoSettings;
        }

        /// <summary>
        /// 登入
        /// </summary>
        /// <param name="loginInfo"></param>
        /// <returns></returns>
        /// <remarks>
        /// Sample response:<br/>
        ///     {
        ///         "accountId": 2,
        ///         "accessToken":"JWT token",
        ///         "refreshToken": "e320d0affae5473da7860d51dce66e43",
        ///         "expireTime": 1723304825,
        ///         "accountStatus": 2  //Unverified = 1, Verified = 2, Blacklist = 3
        ///     }
        /// </remarks>
        /// <response code ="200">登入成功</response>
        /// <response code ="400">帳號或密碼有誤，請重新輸入</response>
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO loginInfo)
        {
            var authRes = await _userManageService.AuthenticateUser(loginInfo.Email, loginInfo.Password);
            if (authRes.IsValid)
            {
                return Ok(await _jwtService.GenerateToken(new GenerateTokenDTO 
                                                { AccountId = authRes.AccountId,
                                                  Email = loginInfo.Email
                                                }));
            }
            return BadRequest("帳號或密碼有誤，請重新輸入");
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout(LogoutRequest request)
        {
            await _jwtService.LogoutAsync(request);
            return Ok();
        }

        /// <summary>
        /// 更新AccessToken
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code ="401"></response>
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
        {
            try
            {
                var response = await _jwtService.RefreshTokenAsync(request);
                return Ok(response);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Unauthorized();
            }

        }
    }
}
