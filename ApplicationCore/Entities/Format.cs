using System;
using System.Collections.Generic;

namespace ApplicationCore.Entities;

public partial class Format
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int Order { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
