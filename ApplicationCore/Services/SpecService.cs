using ApplicationCore.DTOs.SpecDTOs;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Microsoft.Extensions.Logging;

namespace ApplicationCore.Services
{
    public class SpecService : ISpecService
    {
        private readonly IRepository<Spec> _specRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly ILogger<SpecService> _logger;

        public SpecService(IRepository<Spec> specRepository, ILogger<SpecService> logger, IRepository<Product> productRepository)
        {
            _specRepository = specRepository;
            _logger = logger;
            _productRepository = productRepository;
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
            catch (Exception ex)
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
                _logger.LogError(ex, ex.Message);
                return new OperationResult("商品規格刪除失敗");
            }
        }

        public async Task<OperationResult> UpdateSpecOrderAsync(UpdateSpecOrderRequest request)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(request.ProductId);
                if (product == null || product.IsDelete) return new OperationResult("ProductId無法找到對應的商品");

                var specs = await _specRepository.ListAsync(s => s.ProductId == request.ProductId);
                if (specs == null) return new OperationResult("目前尚未建立對應的商品規格");

                var specIds = specs.Select(s => s.Id);
                var intersectList = request.SpecIdList.Intersect(specIds).ToList();
                if (specs.Count != intersectList.Count) return new OperationResult("參數異常");

                foreach (var spec in specs)
                {
                    spec.Order = request.SpecIdList.IndexOf(spec.Id);
                }
                await _specRepository.UpdateRangeAsync(specs);

                return new OperationResult();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, ex.Message);
                return new OperationResult("商品順序更新失敗");
            }
        }
    }
}
