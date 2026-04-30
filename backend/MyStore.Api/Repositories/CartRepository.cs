using Microsoft.EntityFrameworkCore;
using MyStore.Api.Data;
using MyStore.Api.Models.Entities;

namespace MyStore.Api.Repositories;

public class CartRepository : Repository<CartItem>, ICartRepository
{
    public CartRepository(AppDbContext db) : base(db) { }

    public Task<List<CartItem>> ListByUserAsync(int userId) =>
        Set.Include(c => c.Product).Where(c => c.UserId == userId).ToListAsync();

    public Task<CartItem?> GetByUserAndProductAsync(int userId, int productId) =>
        Set.FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

    public async Task ClearAsync(int userId)
    {
        var items = await Set.Where(c => c.UserId == userId).ToListAsync();
        Set.RemoveRange(items);
    }
}
