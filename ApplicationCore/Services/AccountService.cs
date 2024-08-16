using ApplicationCore.DTOs;
using ApplicationCore.DTOs.AccountDTOs;
using ApplicationCore.Entities;
using ApplicationCore.Enums;
using ApplicationCore.Helpers;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Settings;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Web;

namespace ApplicationCore.Services
{
    public class AccountService : IAccountService
    {
        private readonly IRepository<Account> _accountRepo;
        private readonly IApplicationPasswordHasher _applicationPasswordHasher;
        private const string passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\W).{6,20}$";
        private readonly IEmailSender _emailSender;
        private readonly ILogger<AccountService> _logger;
        private readonly SendEmailSettings _sendEmailSettins;

        public AccountService(IRepository<Account> accountRepo, IApplicationPasswordHasher applicationPasswordHasher, IEmailSender emailSender, ILogger<AccountService> logger, SendEmailSettings sendEmailSettings)
        {
            _accountRepo = accountRepo;
            _applicationPasswordHasher = applicationPasswordHasher;
            _emailSender = emailSender;
            _logger = logger;
            _sendEmailSettins = sendEmailSettings;
        }

        private class VerifyEmailDTO
        {
            public int AccountId { get; set; }
            public DateTimeOffset ExpireTime { get; set; }
        }

        private class ForgetPasswordDTO
        {
            public int AccountId { get; set; }
            public DateTimeOffset ExpireTime { get; set; }
        }


        public async Task<OperationResult<CreateAccountResponse>> CreateAccount(RegistDTO registInfo)
        {
            if (await IsEmailExist(registInfo.Email) || await IsMobileExist(registInfo.Mobile))
            {
                return new OperationResult<CreateAccountResponse>("此帳號已註冊過");
            }
            if (string.IsNullOrEmpty(registInfo.Name))
            {
                return new OperationResult<CreateAccountResponse>("請輸入姓名");
            }
            if (!ValidateHelper.TryValidateMobile(registInfo.Mobile, out var mobileErrorMsg))
            {
                return new OperationResult<CreateAccountResponse>(mobileErrorMsg);
            }
            if (!ValidateHelper.TryValidateEmail(registInfo.Email, out var emailErrorMsg))
            {
                return new OperationResult<CreateAccountResponse>(emailErrorMsg);
            }
            if (!Regex.IsMatch(registInfo.Password, passwordPattern))
            {
                return new OperationResult<CreateAccountResponse>("密碼格式有誤");
            }
            if (registInfo.Password != registInfo.ConfirmedPassword)
            {
                return new OperationResult<CreateAccountResponse>("密碼與確認密碼不符");
            }

            var account = new Account
            {
                Name = registInfo.Name,
                Email = registInfo.Email.ToLower(),
                Mobile = registInfo.Mobile,
                Password = _applicationPasswordHasher.HashPassword(registInfo.ConfirmedPassword),
                CreateAt = DateTimeOffset.UtcNow,
                Status = AccountStatus.Unverified
            };
            _accountRepo.Add(account);

            var result = new CreateAccountResponse()
            {
                Email = registInfo.Email,
                Name = registInfo.Name,
                AccountId = account.Id
            };

            await SendVerifyEmailHandler(account);

            return new OperationResult<CreateAccountResponse>(result);
        }

        /// <summary>
        /// 寄發驗證信
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        private async Task SendVerifyEmailHandler(Account account)
        {
            var verifyDTO = new VerifyEmailDTO()
            {
                AccountId = account.Id,
                ExpireTime = DateTimeOffset.UtcNow.AddMinutes(15),
            };
            string serializeStr = SerializeInput(verifyDTO);

            var uri = new Uri(_sendEmailSettins.VerifyEmailReturnURL).GetLeftPart(UriPartial.Path);
            //QueryHelpers.AddQueryString 回傳 URLEncode後的結果
            var returnURL = QueryHelpers.AddQueryString(uri, "p", serializeStr);

            var mailTemplate = EmailTemplateHelper.SignupEmailTemplate(account.Name, returnURL);
            await _emailSender.SendAsync(new EmailDTO
            {
                MailTo = account.Name,
                MailToEmail = account.Email,
                Subject = "EleganceParadis 註冊驗證信",
                HTMLContent = mailTemplate
            });
        }


        /// <summary>
        /// 將物件序列化為base64 string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string SerializeInput<T>(T input) where T : class
        {
            var byteArr = JsonSerializer.SerializeToUtf8Bytes(input);
            var base64Str = Convert.ToBase64String(byteArr);
            return base64Str;
        }

        public async Task<OperationResult<VerifyEmailResponse>> VerifyEmailAsync(string encodingParameter)
        {
            try
            {
                var verifyDTO = DeserializeURLEncodeParameter<VerifyEmailDTO>(encodingParameter);

                if (verifyDTO == null)
                    return new OperationResult<VerifyEmailResponse>("註冊驗證參數異常");

                if (verifyDTO.ExpireTime.CompareTo(DateTimeOffset.UtcNow) < 0)
                    return new OperationResult<VerifyEmailResponse>("註冊驗證逾時");

                var account = await _accountRepo.GetByIdAsync(verifyDTO.AccountId);
                account.Status = AccountStatus.Verified;
                await _accountRepo.UpdateAsync(account);

                return new OperationResult<VerifyEmailResponse>()
                {
                    IsSuccess = true,
                    ResultDTO = new VerifyEmailResponse()
                    {
                        AccountId = account.Id,
                        Email = account.Email
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new OperationResult<VerifyEmailResponse>("註冊驗證失敗");
            }
        }

        /// <summary>
        /// 反序列化base64 string to Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="base64Str"></param>
        /// <returns></returns>
        private T DeserializeParameter<T>(string base64Str) where T : class
        {
            try
            {
                var byteArr = Convert.FromBase64String(base64Str);
                var output = JsonSerializer.Deserialize<T>(byteArr);
                return output;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        private T DeserializeURLEncodeParameter<T>(string encodeParameter) where T : class
        {
            var decode = HttpUtility.UrlDecode(encodeParameter);
            return DeserializeParameter<T>(decode);
        }


        public async Task<GetAccountInfoResponse> GetAccountInfo(int accountId)
        {
            var result = await _accountRepo.GetByIdAsync(accountId);
            if (result == null) return default;
            return new GetAccountInfoResponse()
            {
                AccountId = accountId,
                Email = result.Email,
                Name = result.Name,
                Mobile = result.Mobile
            };
        }

        private async Task<bool> IsEmailExist(string email)
        {
            return await _accountRepo.AnyAsync(x => x.Email.ToLower() == email.ToLower());
        }

        private async Task<bool> IsEmailExist(Account account, string email)
        {
            if (account.Email.ToLower() == email.ToLower()) return false;
            return await IsEmailExist(email);
        }

        private async Task<bool> IsMobileExist(string mobile)
        {
            return await _accountRepo.AnyAsync(x => x.Mobile == mobile);
        }

        private async Task<bool> IsMobileExist(Account account, string mobile)
        {
            if (account.Mobile == mobile) return false;
            return await IsMobileExist(mobile);
        }

        public async Task<OperationResult<UpdateAcoountInfoResult>> UpdateAccountInfo(UpdateAccountInfoRequest accountInfo)
        {
            if (string.IsNullOrEmpty(accountInfo.Name))
            {
                return new OperationResult<UpdateAcoountInfoResult>("請輸入名字");
            }
            if (!ValidateHelper.TryValidateEmail(accountInfo.Email, out var emailErrorMsg))
            {
                return new OperationResult<UpdateAcoountInfoResult>(emailErrorMsg);
            }
            if (!ValidateHelper.TryValidateMobile(accountInfo.Mobile, out var mobileErrorMsg))
            {
                return new OperationResult<UpdateAcoountInfoResult>(mobileErrorMsg);
            }
            var account = await _accountRepo.GetByIdAsync(accountInfo.AccountId);

            if (account == null) return new OperationResult<UpdateAcoountInfoResult>("查無此人");

            if (await IsEmailExist(account, accountInfo.Email))
            {
                return new OperationResult<UpdateAcoountInfoResult>("此電子信箱已註冊過");
            }
            if (await IsMobileExist(account, accountInfo.Mobile))
            {
                return new OperationResult<UpdateAcoountInfoResult>("此手機號碼已註冊過");
            }
            account.Email = accountInfo.Email.ToLower();
            account.Mobile = accountInfo.Mobile;
            account.Name = accountInfo.Name;

            var updatedInfo = await _accountRepo.UpdateAsync(account);
            var updateResult = new UpdateAcoountInfoResult
            {
                AccountId = updatedInfo.Id,
                Email = updatedInfo.Email,
                Name = updatedInfo.Name,
                Mobile = updatedInfo.Mobile
            };
            return new OperationResult<UpdateAcoountInfoResult>(updateResult);
        }

        public async Task<OperationResult<UpdateAccountPasswordResult>> UpdateAccountPassword(UpdateAccountPasswordRequest accountInfo)
        {
            if (accountInfo.OldPassword == accountInfo.NewPassword)
                return new OperationResult<UpdateAccountPasswordResult>("新密碼與舊密碼不可相同");

            if (!Regex.IsMatch(accountInfo.NewPassword, passwordPattern))
                return new OperationResult<UpdateAccountPasswordResult>("密碼格式有誤");

            var account = await _accountRepo.GetByIdAsync(accountInfo.AccountId);

            if (account == null)
                return new OperationResult<UpdateAccountPasswordResult>("查無此人");

            if (!_applicationPasswordHasher.VerifyPassword(account.Password, accountInfo.OldPassword))
                return new OperationResult<UpdateAccountPasswordResult>("舊密碼有誤");

            account.Password = _applicationPasswordHasher.HashPassword(accountInfo.NewPassword);

            var updatedInfo = await _accountRepo.UpdateAsync(account);
            var updatedResult = new UpdateAccountPasswordResult
            {
                AccountId = accountInfo.AccountId,
            };
            return new OperationResult<UpdateAccountPasswordResult>(updatedResult);
        }

        public async Task<OperationResult> ForgetPasswordAsync(string email)
        {
            try
            {
                var account = await _accountRepo.FirstOrDefaultAsync(x => x.Email == email);
                if (account == null) return new OperationResult("找不到對應的AccountId");

                var forgetPasswordDTO = new ForgetPasswordDTO
                {
                    AccountId = account.Id,
                    ExpireTime = DateTimeOffset.UtcNow.AddMinutes(15),
                };

                var urlEncode = SerializeInput(forgetPasswordDTO);

                var uri = new Uri(_sendEmailSettins.ForgetPasswordReturnURL).GetLeftPart(UriPartial.Path);
                var returnURL = QueryHelpers.AddQueryString(uri, "p", urlEncode);

                var mailTemplate = EmailTemplateHelper.ForgetPasswordEmailTemplate(account.Name, returnURL);
                await _emailSender.SendAsync(new EmailDTO
                {
                    MailTo = account.Name,
                    MailToEmail = email,
                    Subject = "重設密碼驗證信",
                    HTMLContent = mailTemplate
                });

                return new OperationResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new OperationResult("重設密碼驗證信寄發失敗");
            }
        }

        public async Task<OperationResult> VerifyForgetPasswordAsync(string encodingParameter)
        {
            var dto = DeserializeURLEncodeParameter<ForgetPasswordDTO>(encodingParameter);

            if (dto == null) return new OperationResult("重設密碼參數異常");
            if (dto.ExpireTime.CompareTo(DateTimeOffset.UtcNow) < 0) return new OperationResult("重設密碼逾時");

            var account = await _accountRepo.GetByIdAsync(dto.AccountId);
            if (account == null) return new OperationResult("無法找到對應的AccountId");

            return new OperationResult();
        }

        public async Task<OperationResult> ResetAccountPasswordAsync(ResetAccountPasswordRequest request)
        {
            try
            {
                var verifyResult = await VerifyForgetPasswordAsync(request.EncodingParameter);
                if (!verifyResult.IsSuccess)
                    return verifyResult;

                if (!Regex.IsMatch(request.NewPassword, passwordPattern))
                    return new OperationResult("密碼格式有誤");

                var accountId = DeserializeURLEncodeParameter<ForgetPasswordDTO>(request.EncodingParameter).AccountId;

                var account = await _accountRepo.GetByIdAsync(accountId);
                if (account == null) return new OperationResult("找不到對應的AccountId");

                if (_applicationPasswordHasher.VerifyPassword(account.Password, request.NewPassword))
                    return new OperationResult("與舊密碼相同");

                account.Password = _applicationPasswordHasher.HashPassword(request.NewPassword);
                await _accountRepo.UpdateAsync(account);

                return new OperationResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new OperationResult("重設密碼失敗");
            }
        }

        public async Task<OperationResult> ResendVerifyEmailAsync(ResendVerifyEmailRequest request)
        {
            var account = await _accountRepo.FirstOrDefaultAsync(a => a.Email == request.Email);

            if (account == null)
                return new OperationResult("找不到此用戶");

            if (account.Status != AccountStatus.Unverified)
            {
                return new OperationResult("此帳戶已驗證過");
            }
            else
            {
                await SendVerifyEmailHandler(account);
                return new OperationResult();
            }
        }
    }
}
