using ApplicationCore.DTOs.CartDTO;
using ApplicationCore.Enums;
using System;
using System.Collections.Generic;

namespace ApplicationCore.Entities;

public partial class Order
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public string OrderNo { get; set; } = null!;

    public string Purchaser { get; set; } = null!;

    public string PurchaserTel { get; set; } = null!;

    public string PurchaserEmail { get; set; } = null!;

    /// <summary>
    /// 0 = LinePay, 1 = ECPay
    /// </summary>
    public Enums.PaymentType PaymentType { get; set; }

    public OrderStatus OrderStatus { get; set; }

    public DateTimeOffset CreateAt { get; set; }

    public string City { get; set; } = null!;

    public string District { get; set; } = null!;

    public string Address { get; set; } = null!;

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
