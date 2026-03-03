using TaskApp.Application.Users;

namespace TaskApp.Application.Abstractions.Auth
{
    public interface IAuthService
    {
        Task<AuthResultDto> RegisterAsync(string email, string password, string firstName, string lastName, CancellationToken ct);
        Task<AuthResultDto> LoginAsync(string email, string password, CancellationToken ct);
    }
}
