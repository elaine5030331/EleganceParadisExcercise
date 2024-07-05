using System;
using System.Collections.Generic;

namespace ApplicationCore.Entities;

public partial class OrderDetail
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public string ProductName { get; set; } = null!;

    public string Sku { get; set; } = null!;

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public int Sequence { get; set; }

    public decimal? Discount { get; set; }

    public virtual Order Order { get; set; } = null!;
}
