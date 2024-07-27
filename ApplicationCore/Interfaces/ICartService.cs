using ApplicationCore.DTOs.CartDTO;
using ApplicationCore.Models;

namespace ApplicationCore.Interfaces
{
    public interface ICartService
    {
        Task<OperationResult<CartDTO>> AddCartItemAsync(AddCartItemDTO addCartItemDTO);
        Task<OperationResult<CartDTO>> GetCartItemsAsync(int accountId);
        Task<OperationResult<CartDTO>> UpdateCartItemsAsync(int accountId, UpdateCartItemDTO updateCartItemDTO);
        Task<OperationResult<CartDTO>> DeleteCartItemAsync(DeleteCartItemDTO deleteCartItemDTO);
    }
}