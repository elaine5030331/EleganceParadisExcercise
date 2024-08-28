using ApplicationCore.DTOs.AdminDTOs.AccountDTOs;
using ApplicationCore.Entities;
using ApplicationCore.Enums;
using ApplicationCore.Interfaces;
using ApplicationCore.Interfaces.AdminInterfaces;
using ApplicationCore.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Services.AdminServices
{
    public class AdminAccountService : IAdminAccountService
    {
        private readonly IRepository<Account> _accountRepo;
        private readonly ILogger<AdminAccountService> _logger;

        public AdminAccountService(IRepository<Account> accountRepo, ILogger<AdminAccountService> logger)
        {
            _accountRepo = accountRepo;
            _logger = logger;
        }

        public async Task<OperationResult<GetAccountByIdResponse>> GetAccountByIdAsync(int accountId)
        {
            try
            {
                var account = await _accountRepo.GetByIdAsync(accountId);
                if(account == null) 
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
    }
}
