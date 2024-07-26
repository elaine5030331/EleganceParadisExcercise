﻿using ApplicationCore.DTOs.CartDTO;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Microsoft.Extensions.Logging;

namespace ApplicationCore.Services
{
    public class CartService : ICartService
    {
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductImage> _productImageRepository;
        private readonly IRepository<Spec> _specRepository;
        private readonly ILogger<CartService> _logger;

        public CartService(IRepository<Cart> cartRepository, IRepository<Category> categoryRepository, IRepository<Product> productRepository, IRepository<ProductImage> productImageRepository, IRepository<Spec> specRepository, ILogger<CartService> logger)
        {
            _cartRepository = cartRepository;
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _productImageRepository = productImageRepository;
            _specRepository = specRepository;
            _logger = logger;
        }

        public async Task<OperationResult<CartDTO>> AddCartItemAsync(AddCartItemDTO addCartItemDTO)
        {
            var cartList = await _cartRepository.ListAsync(c => c.AccountId == addCartItemDTO.AccountId);

            if (cartList != null && cartList.Select(x => x.SpecId).Contains(addCartItemDTO.SpecId))
            {
                //UPDATE FLOW
            }

            try
            {
                var cartEntity = new Cart()
                {
                    AccountId = addCartItemDTO.AccountId,
                    SpecId = addCartItemDTO.SpecId,
                    Quantity = addCartItemDTO.Quantity
                };
                var result = await _cartRepository.AddAsync(cartEntity);
                cartList.Add(cartEntity);

                var currentCart = await GetCurrentCartItems(cartList);

                return new OperationResult<CartDTO>()
                {
                    IsSuccess = true,
                    ResultDTO = GetCartDTO(addCartItemDTO.AccountId, currentCart)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new OperationResult<CartDTO>()
                {
                    IsSuccess = false,
                    ErrorMessage = "購物車新增失敗",
                    ResultDTO = GetCartDTO(addCartItemDTO.AccountId, await GetCurrentCartItems(cartList))
                };
            }
        }

        public async Task<OperationResult<CartDTO>> GetCartItemsAsync(int accountId)
        {
            try
            {
                var carts = await _cartRepository.ListAsync(x => x.AccountId == accountId);
                var cartItems = carts == null || carts.Count == 0 ? new List<CartItem>() : await GetCurrentCartItems(carts);
                var cartDTO = GetCartDTO(accountId, cartItems);
                return new OperationResult<CartDTO>(cartDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new OperationResult<CartDTO>()
                {
                    IsSuccess = false,
                    ErrorMessage = "取得購物車資料失敗",
                    ResultDTO = GetCartDTO(accountId, new List<CartItem>())
                };
            }
        }

        private CartDTO GetCartDTO(int accountId, List<CartItem> cartItems)
        {
            var shippingFee = cartItems.Any() ? 130 : 0;
            var subTotal = cartItems.Sum(x => x.Quantity * x.UnitPrice);

            return new CartDTO()
            {
                AccountId = accountId,
                CartItems = cartItems,
                ShippingFee = shippingFee,
                SubTotal = subTotal,
                CartTotal = shippingFee + subTotal,
                PaymentTypes = GetPaymentTypes()
            };
        }


        private async Task<List<CartItem>> GetCurrentCartItems(List<Cart> carts)
        {
            var specs = await _specRepository.ListAsync(s => carts.Select(c => c.SpecId).Contains(s.Id));
            var products = await _productRepository.ListAsync(p => specs.Select(s => s.ProductId).Contains(p.Id));
            var categories = await _categoryRepository.ListAsync(c => products.Select(p => p.CategoryId).Contains(c.Id));
            var productImages = await _productImageRepository.ListAsync(pi => products.Select(p => p.Id).Contains(pi.ProductId));

            var result = carts.Select(cart =>
            {
                var spec = specs.FirstOrDefault(s => s.Id == cart.SpecId);
                var product = products.FirstOrDefault(p => p.Id == spec.ProductId);
                var category = categories.FirstOrDefault(c => c.Id == product.CategoryId);
                var productImage = productImages.Where(pi => pi.ProductId == product.Id).OrderBy(x => x.Order).FirstOrDefault();
                return new CartItem
                {
                    SpecId = cart.SpecId,
                    CategoryName = category?.Name ?? string.Empty,
                    ProductName = product?.ProductName ?? string.Empty,
                    ProductImage = productImage?.Url ?? string.Empty,//商品圖預設路徑
                    SpecName = spec?.SpecName ?? string.Empty,
                    UnitPrice = spec?.UnitPrice ?? 0,
                    Quantity = cart.Quantity
                };
            }).ToList();
            return result;
        }

        private List<PaymentType> GetPaymentTypes()
        {
            return new List<PaymentType>
            {
                new PaymentType()
                {
                    Type = PaymentTypes.EasyPay,
                    DisplayName = "綠界金流",
                    Icon = string.Empty
                }
            };
        }
    }
}
