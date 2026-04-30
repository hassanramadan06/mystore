using MyStore.Api.Models.Entities;

namespace MyStore.Api.Repositories;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetBySlugAsync(string slug);
}
