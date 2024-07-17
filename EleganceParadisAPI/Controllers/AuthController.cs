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
            var IsUserValid = await _userManageService.AuthenticateUser(loginInfo.UserName, loginInfo.Password);
            if (IsUserValid)
            {
                var roleList = new List<string>() { "Admin", "User"};
                return Ok(_jWT.GenerateToken(loginInfo.UserName, roleList));
            }
            return BadRequest("帳號或密碼有誤，請重新輸入");
        }

        [HttpGet("GetUser")]
        [Authorize(Roles = "Admin, User")]
        public IActionResult GetUser() { 
            return Ok(User.Identity.Name);
        }
    }

    public class LoginDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
