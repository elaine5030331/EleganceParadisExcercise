using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IAccountRepository : IRepository<Account>
    {
        Task<List<Account>> GetAllAsync();
    }
}
