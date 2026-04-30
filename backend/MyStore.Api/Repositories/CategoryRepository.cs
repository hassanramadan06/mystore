using Microsoft.EntityFrameworkCore;
using MyStore.Api.Data;
using MyStore.Api.Models.Entities;

namespace MyStore.Api.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext db) : base(db) { }

    public Task<Category?> GetBySlugAsync(string slug) =>
        Set.FirstOrDefaultAsync(c => c.Slug == slug);
}
