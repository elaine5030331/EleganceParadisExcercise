using ApplicationCore.DTOs.AccountDTOs;
using ApplicationCore.DTOs.AdminAccountDTOs;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IAdminAccountService
    {
        Task<List<GetAllAccountsResponse>> GetAllAccountsAsync();
        Task<OperationResult<GetAccountByIdResponse>> GetAccountByIdAsync(int accountId);
        Task<OperationResult> UpdateAccountInfoAsync(UpdateAdminAccountInfoRequest request);
    }
}
