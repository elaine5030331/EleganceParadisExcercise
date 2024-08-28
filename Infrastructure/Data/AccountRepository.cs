using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class AccountRepository : EFRepository<Account>, IAccountRepository
    {
        //private readonly EleganceParadisContext _context;

        public AccountRepository(EleganceParadisContext context) : base(context) 
        {
            //_context = context;
        }

        public async Task<List<Account>> GetAllAsync()
        {
            //return await base.DbContext.Accounts.ToListAsync();
            return await DbSet.ToListAsync();
        }
    }
}
