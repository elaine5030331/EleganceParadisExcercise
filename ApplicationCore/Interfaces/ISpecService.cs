using ApplicationCore.DTOs.SpecDTOs;
using ApplicationCore.Models;

namespace ApplicationCore.Interfaces
{
    public interface ISpecService
    {
        Task<OperationResult> AddSpecAsync(AddSpecDTO specDTO);
        Task<OperationResult> UpdateSpecAsync(UpdateSpecDTO updateSpecDTO);
        Task<OperationResult> DeleteSpecAsync(int specId);
        Task<OperationResult> UpdateSpecOrderAsync(UpdateSpecOrderRequest request);
    }
}
