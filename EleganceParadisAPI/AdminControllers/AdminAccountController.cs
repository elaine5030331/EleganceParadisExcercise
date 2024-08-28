using ApplicationCore.Constants;
using ApplicationCore.Interfaces.AdminInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EleganceParadisAPI.AdminControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = EleganceParadisRole.Admin)]
    public class AdminAccountController : ControllerBase
    {
        private readonly IAdminAccountService _adminAccountService;

        public AdminAccountController(IAdminAccountService adminAccountService)
        {
            _adminAccountService = adminAccountService;
        }

        /// <summary>
        /// 取得所有會員資料
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Sample response:<br/>
        ///     [
        ///         {
        ///             "accountId": 2,
        ///             "email": "test@gmail.com",
        ///             "accountName": "test1",
        ///             "mobile": "0912345678",
        ///             "createAt": "2024/07/22",
        ///             "status": 2   //Unverified = 1, Verified = 2, Blacklist = 3
        ///         }
        ///     ]
        ///</remarks>
        [HttpGet("GetAllAccounts")]
        public async Task<IActionResult> GetAllAccounts()
        {
            var result = await _adminAccountService.GetAllAccountsAsync();
            return Ok(result);
        }

        /// <summary>
        /// 取得會員資料by accountId
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        /// <remarks>
        /// Sample response:<br/>
        ///     {
        ///         "accountId": 2,
        ///         "email": "test@gmail.com",
        ///         "accountName": "test1",
        ///         "mobile": "0912345678",
        ///         "createAt": "2024/07/22",
        ///         "status": 2   //Unverified = 1, Verified = 2, Blacklist = 3
        ///     }
        /// </remarks>
        /// <response code ="200">成功取得 accountId 的會員資料</response>
        /// <response code ="400">
        /// 1. 找不到對應的AccountId
        /// 2. 取得會員資料失敗
        /// </response>
        [HttpGet("GetAccountById")]
        public async Task<IActionResult> GetAccountById(int accountId)
        {
            var result = await _adminAccountService.GetAccountByIdAsync(accountId);
            if(result.IsSuccess) return Ok(result.ResultDTO);
            return BadRequest(result.ErrorMessage);
        }
    }
}
