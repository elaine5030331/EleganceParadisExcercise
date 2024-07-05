using System;
using System.Collections.Generic;

namespace ApplicationCore.Entities;

public partial class Customer
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Mobile { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual Account IdNavigation { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
