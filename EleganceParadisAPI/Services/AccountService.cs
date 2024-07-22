using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using EleganceParadisAPI.DTOs;
using System.Text.RegularExpressions;
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

        public AccountService(IRepository<Account> accountRepo, IApplicationPasswordHasher applicationPasswordHasher)
        {
            _accountRepo = accountRepo;
            _applicationPasswordHasher = applicationPasswordHasher;
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
            return new OperationResult<CreateAccountResultDTO>(result);

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

    }
}
