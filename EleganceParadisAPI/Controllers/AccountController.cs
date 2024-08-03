﻿using ApplicationCore.Interfaces;
using EleganceParadisAPI.DTOs;
using EleganceParadisAPI.DTOs.AccountDTOs;
using EleganceParadisAPI.Helpers;
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
        private readonly IEmailSender _emailSender;
        private readonly JWTService _jwtHelper;

        public AccountController(AccountService accountService, IEmailSender emailSender, JWTService jwtHelper)
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
        [HttpPut("UpdateCustomerInfo/{id}")]
        public async Task<IActionResult> UpdateAccountInfo(int id, UpdateAccountInfo accountInfo)
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
        [HttpPut("UpdateCustomerPassword/{id}")]
        public async Task<IActionResult> UpdateAccountPassword(int id, UpdateAccountPassword accountInfo)
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
        /// <response code ="400">
        /// 1. 註冊驗證參數異常
        /// 2. 註冊驗證逾時
        /// 3. 註冊驗證失敗
        /// </response>
        /// <remarks>
        /// encodingParameter: API導回前台的Query string
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
        /// <param name="forgetPasswordDTO"></param>
        /// <returns></returns>
        /// <response code ="400">
        /// 1. 找不到對應的AccountId
        /// 2. 重設密碼驗證信寄發失敗
        /// </response>
        [HttpPost("ForgetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordDTO forgetPasswordDTO)
        {
            var result = await _accountService.ForgetPasswordAsync(forgetPasswordDTO.Email);
            if (result.IsSuccess) return Ok();
            return BadRequest(result.ErrorMessage);
        }

        /// <summary>
        /// 重設密碼
        /// </summary>
        /// <param name="resetAccountPasswordDTO"></param>
        /// <returns></returns>
        /// <response code ="400">
        /// 1. 密碼格式有誤
        /// 2. 找不到對應的AccountId
        /// 3. 與舊密碼相同
        /// 4. 重設密碼失敗
        /// </response>
        [HttpPost("ResetAccountPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetAccountPassword(ResetAccountPasswordDTO resetAccountPasswordDTO)
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
