using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using EleganceParadisAPI.DTOs;
using EleganceParadisAPI.Models;
using System.Text.RegularExpressions;

namespace EleganceParadisAPI.Services
{
    public class AccountService
    {
        private readonly IRepository<Account> _accountRepo;
        private readonly IRepository<Customer> _customerRepo;
        private readonly IApplicationPasswordHasher _applicationPasswordHasher;
        private const string passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\W).{6,20}$";
        private const string mobilePattern = @"^09\d{8}$";
        private const string emailPattern = @".*@.*\..*";

        public AccountService(IRepository<Account> accountRepo, IRepository<Customer> customerRepo, IApplicationPasswordHasher applicationPasswordHasher)
        {
            _accountRepo = accountRepo;
            _customerRepo = customerRepo;
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
                Account1 = registInfo.Email,
                Password = _applicationPasswordHasher.HashPassword(registInfo.ConfirmedPassword),
                CreateAt = DateTimeOffset.UtcNow,
                Status = AccountStatus.Unverified,
                Customer = new Customer
                {
                    Name = registInfo.Name,
                    Email = registInfo.Email,
                    Mobile = registInfo.Mobile
                }
            };
            _accountRepo.Add(account);

            var result = new CreateAccountResultDTO()
            {
                Email = registInfo.Email,
                Name = registInfo.Name,
                UserId = account.Id
            };
            return new OperationResult<CreateAccountResultDTO>(result);

        }

        public async Task<GetCustomerInfoDTO> GetCustomerInfo(int customertId)
        {
            var result = await _customerRepo.GetByIdAsync(customertId);
            if (result == null) return default;
            return new GetCustomerInfoDTO()
            {
                CustomerId = customertId,
                Email = result.Email,
                Name = result.Name,
                Mobile = result.Mobile
            };
        }

        private async Task<bool> IsEmailExist(string email)
        {
            return await _customerRepo.AnyAsync(x => x.Email == email);
        }

        private async Task<bool> IsEmailExist(Customer customer, string email)
        {
            if (customer.Email.ToLower() == email.ToLower()) return false;
            return await _customerRepo.AnyAsync(x => x.Email == email);
        }

        private async Task<bool> IsMobileExist(string mobile)
        {
            return await _customerRepo.AnyAsync(x => x.Mobile == mobile);
        }

        private async Task<bool> IsMobileExist(Customer customer, string mobile)
        {
            if (customer.Mobile == mobile) return false;
            return await _customerRepo.AnyAsync(_ => _.Mobile == mobile);
        }

        public async Task<OperationResult<UpdateCustomerResult>> UpdateCustomerInfo(UpdateCustomerInfo customerInfo)
        {
            if (string.IsNullOrEmpty(customerInfo.Name))
            {
                return new OperationResult<UpdateCustomerResult>("請輸入名字");
            }
            if (!Regex.IsMatch(customerInfo.Email, emailPattern))
            {
                return new OperationResult<UpdateCustomerResult>("電子信箱格式有誤");
            }
            if (!Regex.IsMatch(customerInfo.Mobile, mobilePattern))
            {
                return new OperationResult<UpdateCustomerResult>("手機號格式有誤");
            }
            var customer = await _customerRepo.GetByIdAsync(customerInfo.CustomerId);

            if (customer == null) return new OperationResult<UpdateCustomerResult>("查無此人");

            if (await IsEmailExist(customer, customerInfo.Email))
            {
                return new OperationResult<UpdateCustomerResult>("此電子信箱已註冊過");
            }
            if (await IsMobileExist(customer, customerInfo.Mobile))
            {
                return new OperationResult<UpdateCustomerResult>("此手機號碼已註冊過");
            }
            customer.Email = customerInfo.Email;
            customer.Mobile = customerInfo.Mobile;
            customer.Name = customerInfo.Name;

            var updatedInfo = await _customerRepo.UpdateAsync(customer);
            var updateResult = new UpdateCustomerResult
            {
                CustomerId = updatedInfo.Id,
                Email = updatedInfo.Email,
                Name = updatedInfo.Name,
                Mobile = updatedInfo.Mobile
            };
            return new OperationResult<UpdateCustomerResult>(updateResult);
        }

        public async Task<OperationResult<UpdateAccountResult>> UpdateAccountPassword(UpdateAccountPassword accountInfo)
        {
            if (accountInfo.OldPassword == accountInfo.NewPassword)
                return new OperationResult<UpdateAccountResult>("新密碼與舊密碼不可相同");

            if (!Regex.IsMatch(accountInfo.NewPassword, passwordPattern))
                return new OperationResult<UpdateAccountResult>("密碼格式有誤");

            var account = await _accountRepo.GetByIdAsync(accountInfo.AccountId);

            if (account == null) 
                return new OperationResult<UpdateAccountResult>("查無此人");

            if (!_applicationPasswordHasher.VerifyPassword(account.Password, accountInfo.OldPassword))
                return new OperationResult<UpdateAccountResult>("舊密碼有誤");

            account.Password = _applicationPasswordHasher.HashPassword(accountInfo.NewPassword);

            var updatedInfo = await _accountRepo.UpdateAsync(account);
            var updatedResult = new UpdateAccountResult
            {
                AccountId = accountInfo.AccountId,
            };
            return new OperationResult<UpdateAccountResult>(updatedResult);
        }

    }
}
