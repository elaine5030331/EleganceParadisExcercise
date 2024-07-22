using ApplicationCore.DTOs;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public class UserManageService : IUserManageService
    {
        private readonly IRepository<Account> _accountRepo;
        private readonly IApplicationPasswordHasher _applicationPasswordHasher;

        public UserManageService(IRepository<Account> accountRepo, IApplicationPasswordHasher applicationPasswordHasher)
        {
            _accountRepo = accountRepo;
            _applicationPasswordHasher = applicationPasswordHasher;
        }

        public async Task<AuthenticateDTO> AuthenticateUser(string email, string password)
        {
            var result = new AuthenticateDTO()
            {
                AccountId = -1
            };
            var account = await _accountRepo.FirstOrDefaultAsync(x => x.Email == email.ToLower());
            if (account == null) return result;
            if(_applicationPasswordHasher.VerifyPassword(account.Password, password))
            {
                result.AccountId = account.Id;
                result.IsValid = true;
            }
            return result;
        }
    }

}
