using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services;
public class ApplicationPasswordHasherService : IApplicationPasswordHasher
{
    private readonly IPasswordHasher<Account> _passwordHasher;
    private readonly ILogger<ApplicationPasswordHasherService> _logger;

    public ApplicationPasswordHasherService(IPasswordHasher<Account> passwordHasher, ILogger<ApplicationPasswordHasherService> logger)
    {
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public string HashPassword(string password)
    {
        //因HashPassword()需傳入<TUser>，但此參數目前未使用到，若需要帶入user資料記得去override
        var account = new Account();
        return _passwordHasher.HashPassword(account, password);
    }

    public bool VerifyPassword(string hashedPassword, string password)
    {
        var account = new Account();
        var result = _passwordHasher.VerifyHashedPassword(account, hashedPassword, password);
        if(result == PasswordVerificationResult.Failed) return false;
        if(result == PasswordVerificationResult.SuccessRehashNeeded)
        {
            _logger.LogWarning("密碼驗證成功，但密碼使用已被取代的演算法進行編碼，而且應該重新套用並更新");
        }
        return true;
    }
}
