using Microsoft.EntityFrameworkCore;
using MyStore.Api.Data;
using MyStore.Api.Models.Entities;

namespace MyStore.Api.Repositories;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(AppDbContext db) : base(db) { }

    public IQueryable<Order> QueryWithItems() =>
        Set.Include(o => o.Items).ThenInclude(i => i.Product);

    public Task<Order?> GetWithItemsAsync(int id) =>
        QueryWithItems().FirstOrDefaultAsync(o => o.Id == id);

    public Task<List<Order>> ListByUserAsync(int userId) =>
        QueryWithItems().Where(o => o.UserId == userId)
                        .OrderByDescending(o => o.CreatedAt)
                        .ToListAsync();
}
