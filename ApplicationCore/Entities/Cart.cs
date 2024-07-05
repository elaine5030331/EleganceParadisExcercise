using System;
using System.Collections.Generic;

namespace ApplicationCore.Entities;

public partial class Cart
{
    public int Id { get; set; }

    public int SpecId { get; set; }

    public int CustomerId { get; set; }

    public int Quantity { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Spec Spec { get; set; } = null!;
}
