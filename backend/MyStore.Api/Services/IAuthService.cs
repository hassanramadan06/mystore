using MyStore.Api.Models.Dtos;

namespace MyStore.Api.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterDto dto);
    Task<AuthResponse> LoginAsync(LoginDto dto);
}
