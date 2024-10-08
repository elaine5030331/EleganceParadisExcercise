﻿using ApplicationCore.Entities;
using ApplicationCore.Enums;
using ApplicationCore.Models;

namespace ApplicationCore.DTOs.CartDTO
{
    public class CartDTO : BaseOperationResult
    {
        public int AccountId { get; set; }
        public List<CartItem> CartItems { get; set; }
        /// <summary>
        /// 運費
        /// </summary>
        public decimal ShippingFee { get; set; }
        /// <summary>
        /// 商品小計(商品單價 * 數量)
        /// </summary>
        public decimal SubTotal { get; set; }
        /// <summary>
        /// 總額(運費 + 商品小計)
        /// </summary>
        public decimal CartTotal { get; set; }
        public List<PaymentType> PaymentTypes { get; set; }
    }

    public class PaymentType
    {
        public Enums.PaymentType Type { get; set; }
        public string DisplayName { get; set; }
        public string Icon { get; set; }
    }

    public class CartItem
    {
        public int CartId { get; set; }
        public int SelectedSpecId { get; set; }
        public string CategoryName { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string SpecName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public int? Stock { get; set; }
        public List<Specs> Specs { get; set; }
    }

    public class Specs
    {
        public int SpecId { get; set; }
        public string SpecName { get; set; }
        public decimal UnitPrice { get; set; }
        public int? Stock { get; set; }
    }
}
