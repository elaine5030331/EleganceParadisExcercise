using EleganceParadisAPI.DTOs;
using EleganceParadisAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EleganceParadisAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        [HttpPost("CreateAccount")]
        public async Task<IActionResult> CreateAccount(RegistDTO registInfo)
        {
            var result = await _accountService.CreateAccount(registInfo);
            if (result.IsSuccess) return Ok(result.ResultDTO);
            else return BadRequest(result.ErrorMessage);
        }

        /// <summary>
        /// 取得使用者資料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetAccount/{id}")]
        public async Task<IActionResult> GetAccount(int id)
        {
            var result = await _accountService.GetCustomerInfo(id);
            if (result == null) return NotFound();
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
            if (id != customerInfo.CustomerId) return BadRequest();
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
