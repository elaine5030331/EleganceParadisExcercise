using System;
using System.Collections.Generic;

namespace ApplicationCore.Entities;

public partial class AuthTokenHistory
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    /// <summary>
    /// JWT
    /// </summary>
    public string AccessToken { get; set; } = null!;

    public string RefreshToken { get; set; } = null!;

    /// <summary>
    /// RefreshToken過期時間
    /// </summary>
    public DateTimeOffset ExpiredTime { get; set; }

    public DateTimeOffset CreatAt { get; set; }

    public virtual Account Account { get; set; } = null!;
}
