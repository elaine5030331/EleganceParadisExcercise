using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Interfaces.DTOs;
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
        private readonly IRepository<Customer> _customerRepo;
        private readonly IApplicationPasswordHasher _applicationPasswordHasher;

        public UserManageService(IRepository<Account> accountRepo, IRepository<Customer> customerRepo, IApplicationPasswordHasher applicationPasswordHasher)
        {
            _accountRepo = accountRepo;
            _customerRepo = customerRepo;
            _applicationPasswordHasher = applicationPasswordHasher;
        }

        public async Task<AuthenticateDTO> AuthenticateUser(string email, string password)
        {
            var result = new AuthenticateDTO()
            {
                AccountId = -1
            };
            var customer = await _customerRepo.FirstOrDefaultAsync(e => e.Email == email);
            if (customer == null) return result;
            var account = await _accountRepo.FirstOrDefaultAsync(x => x.Id == customer.Id);
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
