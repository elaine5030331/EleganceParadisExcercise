using ApplicationCore.DTOs;
using ApplicationCore.Models;

namespace ApplicationCore.Interfaces
{
    public interface ISpecService
    {
        public Task<OperationResult> AddSpecAsync(AddSpecDTO specDTO);
        public Task<OperationResult> UpdateSpecAsync(UpdateSpecDTO updateSpecDTO);
    }
}
