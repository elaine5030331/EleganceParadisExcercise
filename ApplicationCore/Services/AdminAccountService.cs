using ApplicationCore.DTOs.AdminAccountDTOs;
using ApplicationCore.Entities;
using ApplicationCore.Enums;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public class AdminAccountService : IAdminAccountService
    {
        private readonly IAccountRepository _accountRepo;
        private readonly ILogger<AdminAccountService> _logger;

        public AdminAccountService(IAccountRepository accountRepo, ILogger<AdminAccountService> logger)
        {
            _accountRepo = accountRepo;
            _logger = logger;
        }

        public async Task<List<GetAllAccountsResponse>> GetAllAccountsAsync()
        {
            var accounts = await _accountRepo.GetAllAsync();

            if (accounts == null || accounts.Count < 1)
                return new List<GetAllAccountsResponse>();

            var result = accounts.Select(a => new GetAllAccountsResponse
            {
                AccountId = a.Id,
                AccountName = a.Name,
                Email = a.Email,
                Mobile = a.Mobile,
                CreateAt = a.CreateAt.ToLocalTime().ToString("yyyy/MM/dd"),
                Status = (int)a.Status
            }).ToList();

            return result;
        }

        public async Task<OperationResult<GetAccountByIdResponse>> GetAccountByIdAsync(int accountId)
        {
            try
            {
                var account = await _accountRepo.GetByIdAsync(accountId);
                if (account == null)
                    return new OperationResult<GetAccountByIdResponse>("找不到對應的AccountId");

                return new OperationResult<GetAccountByIdResponse>()
                {
                    IsSuccess = true,
                    ResultDTO = new GetAccountByIdResponse()
                    {
                        AccountId = accountId,
                        AccountName = account.Name,
                        Email = account.Email,
                        Mobile = account.Mobile,
                        CreateAt = account.CreateAt.ToLocalTime().ToString("yyyy/MM/dd"),
                        Status = (int)account.Status
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new OperationResult<GetAccountByIdResponse>("取得會員資料失敗");
            }
        }

        public async Task<OperationResult> UpdateAccountInfoAsync(UpdateAdminAccountInfoRequest request)
        {
            try
            {
                var account = await _accountRepo.GetByIdAsync(request.AccountId);
                if (account == null) return new OperationResult("找不到對應的Account");

                if (!Enum.IsDefined(typeof(AccountStatus), request.Status))
                    return new OperationResult("無此會員狀態");

                account.Status = (AccountStatus)request.Status;
                await _accountRepo.UpdateAsync(account);
                return new OperationResult()
                {
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, ex.Message);
                return new OperationResult("更新會員資料失敗");
            }

        }
    }
}
