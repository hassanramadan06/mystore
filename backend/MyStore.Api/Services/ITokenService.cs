using MyStore.Api.Models.Entities;

namespace MyStore.Api.Services;

public interface ITokenService
{
    string CreateToken(User user);
}
