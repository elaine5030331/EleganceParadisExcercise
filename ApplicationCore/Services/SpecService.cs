﻿using ApplicationCore.DTOs.SpecDTOs;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Microsoft.Extensions.Logging;

namespace ApplicationCore.Services
{
    public class SpecService : ISpecService
    {
        private readonly IRepository<Spec> _specRepository;
        private readonly ILogger<SpecService> _logger;

        public SpecService(IRepository<Spec> specRepository, ILogger<SpecService> logger)
        {
            _specRepository = specRepository;
            _logger = logger;
        }

        public async Task<OperationResult> AddSpecAsync(AddSpecDTO specDTO)
        {
            var spec = new Spec
            {
                ProductId = specDTO.ProductId,
                Sku = specDTO.SKU,
                UnitPrice = specDTO.UnitPrice,
                SpecName = specDTO.SpecName,
                StockQuantity = specDTO.StockQuantity,
                CreateAt = DateTimeOffset.UtcNow
            };

            try
            {
                await _specRepository.AddAsync(spec);
                return new OperationResult();
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, ex.Message);
                return new OperationResult("商品規格新增失敗");
            }
        }

        public async Task<OperationResult> UpdateSpecAsync(UpdateSpecDTO updateSpecDTO)
        {
            try
            {
                var spec = await _specRepository.GetByIdAsync(updateSpecDTO.SpecId);
                if (spec == null) return new OperationResult("找不到對應的商品規格");

                spec.Sku = updateSpecDTO.SKU;
                spec.UnitPrice = updateSpecDTO.UnitPrice;
                spec.SpecName = updateSpecDTO.SpecName;
                spec.Order = updateSpecDTO.Order;
                spec.StockQuantity = updateSpecDTO.StockQuantity;

                await _specRepository.UpdateAsync(spec);
                return new OperationResult();
            }
            catch(Exception ex)
            {
                return new OperationResult("商品規格更新失敗");
            }
        }

        public async Task<OperationResult> DeleteSpecAsync(int specId)
        {
            try
            {
                var spec = await _specRepository.GetByIdAsync(specId);
                if (spec == null) return new OperationResult("找不到對應的商品規格");

                await _specRepository.DeleteAsync(spec);
                return new OperationResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,ex.Message);
                return new OperationResult("商品規格刪除失敗");
            }
        }
    }
}
