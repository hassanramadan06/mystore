using MyStore.Api.Models.Dtos;

namespace MyStore.Api.Services;

public interface ICartService
{
    Task<CartDto> GetAsync(int userId);
    Task<CartDto> AddAsync(int userId, AddToCartDto dto);
    Task<CartDto> UpdateAsync(int userId, int productId, UpdateCartItemDto dto);
    Task<CartDto> RemoveAsync(int userId, int productId);
    Task<CartDto> ClearAsync(int userId);
}
