using System;
using System.Collections.Generic;

namespace ApplicationCore.Entities;

public partial class Spec
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string Sku { get; set; } = null!;

    public decimal UnitPrice { get; set; }

    /// <summary>
    /// 50ml, 75ml, 1 peice
    /// </summary>
    public string SpecName { get; set; } = null!;

    public int Order { get; set; }

    public int? StockQuantity { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual Product Product { get; set; } = null!;
}
