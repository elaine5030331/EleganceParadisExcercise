﻿using ApplicationCore.DTOs;
using ApplicationCore.Entities;
using ApplicationCore.Helpers;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using EleganceParadisAPI.DTOs;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Web;
using static ApplicationCore.Entities.Account;

namespace EleganceParadisAPI.Services
{
    public class AccountService
    {
        private readonly IRepository<Account> _accountRepo;
        private readonly IApplicationPasswordHasher _applicationPasswordHasher;
        private const string passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\W).{6,20}$";
        private const string mobilePattern = @"^09\d{8}$";
        private const string emailPattern = @".*@.*\..*";
        private readonly IEmailSender _emailSender;
        private readonly ILogger<AccountService> _logger;

        public AccountService(IRepository<Account> accountRepo, IApplicationPasswordHasher applicationPasswordHasher, IEmailSender emailSender, ILogger<AccountService> logger)
        {
            _accountRepo = accountRepo;
            _applicationPasswordHasher = applicationPasswordHasher;
            _emailSender = emailSender;
            _logger = logger;
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


        public async Task<OperationResult<CreateAccountResultDTO>> CreateAccount(RegistDTO registInfo)
        {
            if (await IsEmailExist(registInfo.Email) || await IsMobileExist(registInfo.Mobile))
            {
                return new OperationResult<CreateAccountResultDTO>("此帳號已註冊過");
            }
            if (string.IsNullOrEmpty(registInfo.Name))
            {
                return new OperationResult<CreateAccountResultDTO>("請輸入姓名");
            }
            if (!Regex.IsMatch(registInfo.Mobile, mobilePattern))
            {
                return new OperationResult<CreateAccountResultDTO>("電話號碼格式有誤");
            }
            if (!Regex.IsMatch(registInfo.Email, emailPattern))
            {
                return new OperationResult<CreateAccountResultDTO>("電子郵件格式有誤");
            }
            if (!Regex.IsMatch(registInfo.Password, passwordPattern))
            {
                return new OperationResult<CreateAccountResultDTO>("密碼格式有誤");
            }
            if (registInfo.Password != registInfo.ConfirmedPassword)
            {
                return new OperationResult<CreateAccountResultDTO>("密碼與確認密碼不符");
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

            var result = new CreateAccountResultDTO()
            {
                Email = registInfo.Email,
                Name = registInfo.Name,
                AccountId = account.Id
            };

            await SendVerifyEmailHandler(registInfo, account);

            return new OperationResult<CreateAccountResultDTO>(result);
        }

        /// <summary>
        /// 寄發驗證信
        /// </summary>
        /// <param name="registInfo"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        private async Task SendVerifyEmailHandler(RegistDTO registInfo, Account account)
        {
            var verifyDTO = new VerifyEmailDTO()
            {
                AccountId = account.Id,
                ExpireTime = DateTimeOffset.UtcNow.AddMinutes(15),
            };
            string urlEncode = SerializeInput(verifyDTO);

            //TODO:是否需要作為參數
            var returnURL = $"https://localhost:7100/api/account/VerifyEmail/{urlEncode}";
            var mailTemplate = EmailTemplateHelper.SignupEmailTemplate(registInfo.Name, returnURL);
            await _emailSender.SendAsync(new EmailDTO
            {
                MailTo = registInfo.Name,
                MailToEmail = registInfo.Email,
                Subject = "EleganceParadis 註冊驗證信",
                HTMLContent = mailTemplate
            });
        }

        private static string SerializeInput<T>(T input) where T : class
        {
            var byteArr = JsonSerializer.SerializeToUtf8Bytes(input);
            var base64Str = Convert.ToBase64String(byteArr);
            var urlEncode = HttpUtility.UrlEncode(base64Str);
            return urlEncode;
        }

        public async Task<OperationResult> VerifyEmailAsync(string encodingParameter)
        {
            try
            {
                VerifyEmailDTO verifyDTO = DeserializeParameter<VerifyEmailDTO>(encodingParameter);

                if (verifyDTO == null)
                    return new OperationResult("註冊驗證參數異常");

                if (verifyDTO.ExpireTime.CompareTo(DateTimeOffset.UtcNow) < 0)
                    return new OperationResult("註冊驗證逾時");

                var account = await _accountRepo.GetByIdAsync(verifyDTO.AccountId);
                account.Status = AccountStatus.Verified;
                await _accountRepo.UpdateAsync(account);
                return new OperationResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new OperationResult("註冊驗證失敗");
            }
        }

        private T DeserializeParameter<T>(string encodingParameter) where T : class
        {
            try
            {
                var byteArr = Convert.FromBase64String(encodingParameter);
                var output = JsonSerializer.Deserialize<T>(byteArr);
                return output;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public async Task<GetAccountInfoDTO> GetAccountInfo(int accountId)
        {
            var result = await _accountRepo.GetByIdAsync(accountId);
            if (result == null) return default;
            return new GetAccountInfoDTO()
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

        public async Task<OperationResult<UpdateAcoountInfoResult>> UpdateAccountInfo(UpdateAccountInfo accountInfo)
        {
            if (string.IsNullOrEmpty(accountInfo.Name))
            {
                return new OperationResult<UpdateAcoountInfoResult>("請輸入名字");
            }
            if (!Regex.IsMatch(accountInfo.Email, emailPattern))
            {
                return new OperationResult<UpdateAcoountInfoResult>("電子信箱格式有誤");
            }
            if (!Regex.IsMatch(accountInfo.Mobile, mobilePattern))
            {
                return new OperationResult<UpdateAcoountInfoResult>("手機號格式有誤");
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

        public async Task<OperationResult<UpdateAccountPasswordResult>> UpdateAccountPassword(UpdateAccountPassword accountInfo)
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
                //TODO:重設密碼流程確認
                var returnURL = $"https://localhost:7100/api/account/ForgetPassword/{urlEncode}";
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
            var dto = DeserializeParameter<ForgetPasswordDTO>(encodingParameter);

            if (dto == null) return new OperationResult("重設密碼參數異常");
            if (dto.ExpireTime.CompareTo(DateTimeOffset.UtcNow) < 0) return new OperationResult("重設密碼逾時");

            var account = await _accountRepo.GetByIdAsync(dto.AccountId);
            if (account == null) return new OperationResult("無法找到對應的AccountId");

            return new OperationResult();
        }

        public async Task<OperationResult> ResetAccountPasswordAsync(int accountId, string newPassword)
        {
            try
            {
                if (!Regex.IsMatch(newPassword, passwordPattern))
                    return new OperationResult("密碼格式有誤");

                var account = await _accountRepo.GetByIdAsync(accountId);
                if (account == null) return new OperationResult("找不到對應的AccountId");

                if (_applicationPasswordHasher.VerifyPassword(account.Password, newPassword))
                    return new OperationResult("與舊密碼相同");

                account.Password = _applicationPasswordHasher.HashPassword(newPassword);
                await _accountRepo.UpdateAsync(account);

                return new OperationResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new OperationResult("重設密碼失敗");
            }
        }
    }
}
