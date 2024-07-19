﻿using ApplicationCore.DTOs;
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
            var specList = await _specRepository.ListAsync(s => s.ProductId == specDTO.ProductId);
            var lastSpec = specList.OrderByDescending(s => s.Order).FirstOrDefault();
            var lastSpecOrder = lastSpec == null ? 1 : (lastSpec.Order + 1);
            
            var spec = new Spec
            {
                ProductId = specDTO.ProductId,
                Sku = specDTO.SKU,
                UnitPrice = specDTO.UnitPrice,
                SpecName = specDTO.SpecName,
                Order = lastSpecOrder,
                StockQuantity = specDTO.StockQuantity
            };

            try
            {
                await _specRepository.AddAsync(spec);
                return new OperationResult();
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, ex.Message);
                return new OperationResult("產品規格新增失敗");
            }
        }

        public async Task<OperationResult> UpdateSpecAsync(UpdateSpecDTO updateSpecDTO)
        {
            var spec = await _specRepository.GetByIdAsync(updateSpecDTO.SpecId);

            spec.Sku = updateSpecDTO.SKU;
            spec.UnitPrice = updateSpecDTO.UnitPrice;
            spec.SpecName = updateSpecDTO.SpecName;
            spec.Order = updateSpecDTO.Order;
            spec.StockQuantity = updateSpecDTO.StockQuantity;

            try
            {
                await _specRepository.UpdateAsync(spec);
                return new OperationResult();
            }
            catch(Exception ex)
            {
                return new OperationResult("產品規格更新失敗");
            }
        }

        public async Task<OperationResult> DeleteSpecAsync(int specId)
        {
            var spec = await _specRepository.GetByIdAsync(specId);
            try
            {
                await _specRepository.DeleteAsync(spec);
                return new OperationResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,ex.Message);
                return new OperationResult("產品規格刪除失敗");
            }
        }
    }
}
