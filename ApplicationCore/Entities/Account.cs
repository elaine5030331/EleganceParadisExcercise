using System;
using System.Collections.Generic;

namespace ApplicationCore.Entities;

public partial class Account
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Mobile { get; set; } = null!;

    public DateTimeOffset CreateAt { get; set; }

    /// <summary>
    /// EX：黑名單、是否被驗證
    /// </summary>
    public AccountStatus Status { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public enum AccountStatus
    {
        Unverified = 1,
        Verified = 2,
        Blacklist = 3
    }
}
