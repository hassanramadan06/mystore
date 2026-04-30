using Microsoft.EntityFrameworkCore;
using MyStore.Api.Data;
using MyStore.Api.Models.Entities;

namespace MyStore.Api.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext db) : base(db) { }

    public IQueryable<Product> QueryWithCategory() => Set.Include(p => p.Category);

    public Task<Product?> GetWithCategoryAsync(int id) =>
        Set.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
}
