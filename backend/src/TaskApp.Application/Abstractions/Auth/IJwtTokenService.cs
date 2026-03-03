using TaskApp.Domain.Entities;

namespace TaskApp.Application.Abstractions.Auth
{
    public interface IJwtTokenService
    {
        Task<(string token, int expiresInSeconds)> CreateTokenAsync(AppUser user);
    }
}
