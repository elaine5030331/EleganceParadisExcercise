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
        private readonly JWTHelper _jWT;
        private readonly IUserManageService _userManageService;

        public AuthController(JWTHelper jWT, IUserManageService userManageService)
        {
            _jWT = jWT;
            _userManageService = userManageService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO loginInfo)
        {
            var authRes = await _userManageService.AuthenticateUser(loginInfo.Email, loginInfo.Password);
            if (authRes.IsValid)
            {
                return Ok(_jWT.GenerateToken(new GenerateTokenDTO 
                                                { AccountId = authRes.AccountId,
                                                  Email = loginInfo.Email,
                                                  ExpireMinutes = 720
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
