using MyStore.Api.Models.Entities;

namespace MyStore.Api.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetWithCategoryAsync(int id);
    IQueryable<Product> QueryWithCategory();
}
