using MyStore.Api.Models.Entities;

namespace MyStore.Api.Repositories;

public interface ICartRepository : IRepository<CartItem>
{
    Task<List<CartItem>> ListByUserAsync(int userId);
    Task<CartItem?> GetByUserAndProductAsync(int userId, int productId);
    Task ClearAsync(int userId);
}
