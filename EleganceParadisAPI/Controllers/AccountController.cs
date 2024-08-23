using ApplicationCore.DTOs.AccountDTOs;
using ApplicationCore.Interfaces;
using ApplicationCore.Services;
using EleganceParadisAPI.DTOs.AccountDTOs;
using EleganceParadisAPI.Helpers;
using EleganceParadisAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace EleganceParadisAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IEmailSender _emailSender;
        private readonly JWTService _jwtHelper;

        public AccountController(IAccountService accountService, IEmailSender emailSender, JWTService jwtHelper)
        {
            _accountService = accountService;
            _emailSender = emailSender;
            _jwtHelper = jwtHelper;
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
        ///        "confirmedPassword": "Aa*1234" (需與密碼相同)
        ///     }
        /// </remarks>
        /// <response code ="200">新增使用者成功</response>
        /// <response code ="400">
        /// 1. 此帳號已註冊過
        /// 2. 請輸入姓名
        /// 3. 密碼格式有誤
        /// 4. 密碼與確認密碼不符
        /// </response>
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
        /// <response code ="200"></response>
        /// <response code ="400">查無此人</response>
        [HttpGet("GetAccount/{id?}")]
        public async Task<IActionResult> GetAccount(int? id)
        {
            int accountId;


            if (!id.HasValue)
            {
                var getAccountIdRes = User.GetAccountId();
                if (getAccountIdRes == null) return BadRequest("查無此人");
                accountId = getAccountIdRes.Value;
            }
            else
            {
                accountId = id.Value;
            }

            var result = await _accountService.GetAccountInfo(accountId);
            if (result == null) return BadRequest("查無此人");
            return Ok(result);
        }

        /// <summary>
        /// 更新個人資料
        /// </summary>
        /// <param name="accountInfo"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code ="200">更新個人資料成功</response>
        /// <response code ="400">
        /// 1. 請輸入名字
        /// 2. 查無此人
        /// 3. 此電子信箱已註冊過
        /// 4. 此手機號碼已註冊過
        /// </response>
        [HttpPut("UpdateCustomerInfo/{id}")]
        public async Task<IActionResult> UpdateAccountInfo(int id, UpdateAccountInfoRequest accountInfo)
        {
            if (id != accountInfo.AccountId) return BadRequest();
            var result = await _accountService.UpdateAccountInfo(accountInfo);
            if (result.IsSuccess) return Ok(result.ResultDTO);
            else return BadRequest(result.ErrorMessage);
        }

        /// <summary>
        /// 更新使用者密碼
        /// </summary>
        /// <param name="accountInfo"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code ="204">更新使用者密碼成功</response>
        /// <response code ="400">
        /// 1. 新密碼與舊密碼不可相同
        /// 2. 密碼格式有誤
        /// 3. 查無此人
        /// 4. 舊密碼有誤
        /// </response>
        [HttpPut("UpdateCustomerPassword/{id}")]
        public async Task<IActionResult> UpdateAccountPassword(int id, UpdateAccountPasswordRequest accountInfo)
        {
            if (id != accountInfo.AccountId) return BadRequest();
            var result = await _accountService.UpdateAccountPassword(accountInfo);
            if (result.IsSuccess) return NoContent();
            else return BadRequest(result.ErrorMessage);
        }

        /// <summary>
        /// 驗證會員
        /// </summary>
        /// <param name="encodingParameter"></param>
        /// <returns></returns>
        /// <response code ="200">驗證會員成功</response>
        /// <response code ="400">
        /// 1. 註冊驗證參數異常
        /// 2. 註冊驗證逾時
        /// 3. 註冊驗證失敗
        /// </response>
        /// <remarks>
        /// encodingParameter: API導回前台的Query string <br/>
        /// </remarks>
        [HttpGet("VerifyEmail/{encodingParameter}")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyEmail(string encodingParameter)
        {
            var result = await _accountService.VerifyEmailAsync(encodingParameter);
            if (result.IsSuccess)
                return Ok(await _jwtHelper.GenerateToken(new GenerateTokenDTO
                {
                    AccountId = result.ResultDTO.AccountId,
                    Email = result.ResultDTO.Email
                }));
            return BadRequest(result.ErrorMessage);
        }

        /// <summary>
        /// 發送重設密碼信件
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code ="200">發送重設密碼信件成功</response>
        /// <response code ="400">
        /// 1. 找不到對應的AccountId
        /// 2. 重設密碼驗證信寄發失敗
        /// </response>
        [HttpPost("ForgetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordRequest request)
        {
            var result = await _accountService.ForgetPasswordAsync(request.Email);
            if (result.IsSuccess) return Ok();
            return BadRequest(result.ErrorMessage);
        }

        /// <summary>
        /// 重設密碼
        /// </summary>
        /// <param name="resetAccountPasswordDTO"></param>
        /// <returns></returns>
        /// <response code ="200">重設密碼成功</response>
        /// <response code ="400">
        /// 1. 重設密碼參數異常
        /// 2. 重設密碼逾時
        /// 3. 無法找到對應的AccountId
        /// 4. 密碼格式有誤
        /// 5. 與舊密碼相同
        /// 6. 重設密碼失敗
        /// </response>
        [HttpPost("ResetAccountPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetAccountPassword(ResetAccountPasswordRequest resetAccountPasswordDTO)
        {
            //var verifyResult = await _accountService.VerifyForgetPasswordAsync(resetAccountPasswordDTO.EncodingParameter);
            //if (!verifyResult.IsSuccess) 
            //    return BadRequest(verifyResult.ErrorMessage);

            var result = await _accountService.ResetAccountPasswordAsync(resetAccountPasswordDTO);
            
            if (result.IsSuccess) return Ok();
            return BadRequest(result.ErrorMessage);
        }

        /// <summary>
        /// 重發註冊驗證信
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code ="200">信件已寄出</response>
        /// <response code ="400">
        /// 1. 找不到此用戶
        /// 2. 此帳戶已驗證過
        /// 3. 重發註冊驗證信失敗
        /// </response>
        [HttpPost("ResendVerifyEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendVerifyEmail(ResendVerifyEmailRequest request)
        {
            var result = await _accountService.ResendVerifyEmailAsync(request);
            if (result.IsSuccess) return Ok();
            return BadRequest(result.ErrorMessage);
        }
    }   
}
