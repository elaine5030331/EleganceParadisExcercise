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
        private readonly IRepository<Customer> _customerRepo;
        private readonly IApplicationPasswordHasher _applicationPasswordHasher;

        public UserManageService(IRepository<Account> accountRepo, IRepository<Customer> customerRepo, IApplicationPasswordHasher applicationPasswordHasher)
        {
            _accountRepo = accountRepo;
            _customerRepo = customerRepo;
            _applicationPasswordHasher = applicationPasswordHasher;
        }

        public async Task<bool> AuthenticateUser(string email, string password)
        {
            var customer = await _customerRepo.FirstOrDefaultAsync(e => e.Email == email);
            if (customer == null) return false;
            var account = await _accountRepo.FirstOrDefaultAsync(x => x.Id == customer.Id);
            if (account == null) return false;
            return _applicationPasswordHasher.VerifyPassword(account.Password, password);
        }
    }
}
