using EleganceParadisAPI.DTOs;
using EleganceParadisAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EleganceParadisAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly AccountService _accountService;

        public AccountController(AccountService accountService)
        {
            _accountService = accountService;
        }

        /// <summary>
        /// 新增使用者
        /// </summary>
        /// <param name="registInfo"></param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:<br/>
        ///     {
        ///        "name": "ElaineKang", 
        ///        "mobile": "0960123321", (格式：需為09開頭，並且電話號碼長度為10)
        ///        "email": "ek@gmail.com", (格式：需包還 "@" 及 "." 字元)
        ///        "password": "Aa*1234", (格式: 長度為6-20，需包含至少一個英文大寫、英文小寫、數字及符號)
        ///        "comfirmedPassword": "Aa*1234" (需與密碼相同)
        ///     }
        /// </remarks>
        [HttpPost("CreateAccount")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateAccount(RegistDTO registInfo)
        {
            var result = await _accountService.CreateAccount(registInfo);
            if (result.IsSuccess) return Ok(result.ResultDTO);
            else return BadRequest(result.ErrorMessage);
        }

        /// <summary>
        /// 取得使用者資料
        /// </summary>
        /// <param name="id">accountId(可不帶)</param>
        /// <returns></returns>
        [HttpGet("GetAccount/{id?}")]
        public async Task<IActionResult> GetAccount(int? id)
        {
            int accountId;
            if (!id.HasValue)
            {
                var claim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.PrimarySid);
                if (claim == null) return BadRequest("查無此人");

                if (!int.TryParse(claim.Value, out accountId))
                    return BadRequest("查無此人");
            }
            else
            {
                accountId = id.Value;
            }

            var result = await _accountService.GetCustomerInfo(accountId);
            if (result == null) return BadRequest("查無此人");
            return Ok(result);
        }

        /// <summary>
        /// 更新個人資料
        /// </summary>
        /// <param name="customerInfo"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("UpdateCustomerInfo/{id}")]
        public async Task<IActionResult> UpdateCustomerInfo(int id, UpdateCustomerInfo customerInfo)
        {
            if (id != customerInfo.AccountId) return BadRequest();
            var result = await _accountService.UpdateCustomerInfo(customerInfo);
            if (result.IsSuccess) return Ok(result.ResultDTO);
            else return BadRequest(result.ErrorMessage);
        }

        /// <summary>
        /// 更新使用者密碼
        /// </summary>
        /// <param name="accountInfo"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("UpdateCustomerPassword/{id}")]
        public async Task<IActionResult> UpdateAccountPassword(int id, UpdateAccountPassword accountInfo)
        {
            if (id != accountInfo.AccountId) return BadRequest();
            var result = await _accountService.UpdateAccountPassword(accountInfo);
            if (result.IsSuccess) return NoContent();
            else return BadRequest(result.ErrorMessage);
        }
    }
}
