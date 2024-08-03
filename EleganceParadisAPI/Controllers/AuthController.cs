using ApplicationCore.Interfaces;
using EleganceParadisAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EleganceParadisAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JWTService _jWT;
        private readonly IUserManageService _userManageService;

        public AuthController(JWTService jWT, IUserManageService userManageService)
        {
            _jWT = jWT;
            _userManageService = userManageService;
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
        ///         "expireTime": 1723304825
        ///     }
        /// </remarks>
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO loginInfo)
        {
            var authRes = await _userManageService.AuthenticateUser(loginInfo.Email, loginInfo.Password);
            if (authRes.IsValid)
            {
                return Ok(await _jWT.GenerateToken(new GenerateTokenDTO 
                                                { AccountId = authRes.AccountId,
                                                  Email = loginInfo.Email,
                                                  AccessTokenExpireMinutes = 15
                                                }));
            }
            return BadRequest("帳號或密碼有誤，請重新輸入");
        }
    }

    public class LoginDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
