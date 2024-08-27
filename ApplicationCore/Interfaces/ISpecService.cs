using ApplicationCore.DTOs.SpecDTOs;
using ApplicationCore.Models;

namespace ApplicationCore.Interfaces
{
    public interface ISpecService
    {
        public Task<OperationResult> AddSpecAsync(AddSpecDTO specDTO);
        public Task<OperationResult> UpdateSpecAsync(UpdateSpecDTO updateSpecDTO);
        public Task<OperationResult> DeleteSpecAsync(int specId);
    }
}
