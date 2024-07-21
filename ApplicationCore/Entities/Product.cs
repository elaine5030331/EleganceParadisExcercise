using System;
using System.Collections.Generic;

namespace ApplicationCore.Entities;

public partial class Product
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    /// <summary>
    /// 產品編號
    /// </summary>
    public string Spu { get; set; } = null!;

    public string ProductName { get; set; } = null!;

    /// <summary>
    /// 是否上架
    /// </summary>
    public bool Enable { get; set; }

    public bool IsDelete { get; set; }

    public int Order { get; set; }

    public string? Description { get; set; }

    public DateTimeOffset CreateAt { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    public virtual ICollection<Spec> Specs { get; set; } = new List<Spec>();

    public virtual ICollection<Format> Formats { get; set; } = new List<Format>();
}
