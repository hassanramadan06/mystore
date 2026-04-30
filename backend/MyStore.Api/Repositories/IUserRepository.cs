using MyStore.Api.Models.Entities;

namespace MyStore.Api.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
}
