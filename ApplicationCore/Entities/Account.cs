using System;
using System.Collections.Generic;

namespace ApplicationCore.Entities;

public partial class Account
{
    public int Id { get; set; }

    public string Account1 { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateTimeOffset CreateAt { get; set; }

    /// <summary>
    /// EX：黑名單、是否被驗證
    /// </summary>
    public AccountStatus Status { get; set; }

    public virtual Customer? Customer { get; set; }
}

public enum AccountStatus
{
    Unverified = 1,
    Verified = 2,
    Blacklist = 3
}
