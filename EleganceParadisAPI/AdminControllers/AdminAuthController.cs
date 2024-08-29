using ApplicationCore.Constants;
using ApplicationCore.Settings;
using EleganceParadisAPI.DTOs.AuthDTOs;
using EleganceParadisAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EleganceParadisAPI.AdminControllers
{
    [Route("api/AdminAuthController")]
    [ApiController]
    public class AdminAuthController : ControllerBase
    {
        private readonly JWTService _jwtService;
        private readonly AdminInfoSettings _adminInfoSettings;

        public AdminAuthController(JWTService jwtService, AdminInfoSettings adminInfoSettings)
        {
            _jwtService = jwtService;
            _adminInfoSettings = adminInfoSettings;
        }


        /// <summary>
        /// 後台登入
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code ="200">後台登入成功</response>
        /// <response code ="400">帳號或密碼有誤，請重新輸入</response>
        [HttpPost("AdminLogin")]
        public IActionResult AdminLogin(AdminLoginRequest request)
        {
            var accountName = _adminInfoSettings.AccountName;
            var password = _adminInfoSettings.Password;

            if (accountName != request.AccountName || password != request.Password)
                return BadRequest("帳號或密碼有誤，請重新輸入");

            return Ok(_jwtService.GenerateAdminToken(accountName));
        }
    }
}
