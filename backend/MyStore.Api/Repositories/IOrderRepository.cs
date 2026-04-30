using MyStore.Api.Models.Entities;

namespace MyStore.Api.Repositories;

public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetWithItemsAsync(int id);
    Task<List<Order>> ListByUserAsync(int userId);
    IQueryable<Order> QueryWithItems();
}
