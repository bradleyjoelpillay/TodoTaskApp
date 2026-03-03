using Microsoft.AspNetCore.Identity;
using TaskApp.Application.Abstractions.Auth;
using TaskApp.Application.Users;
using TaskApp.Domain.Entities;

namespace TaskApp.Infrastructure.Auth
{
    public sealed class AuthService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IJwtTokenService jwt) : IAuthService
    {
        public async Task<AuthResultDto> RegisterAsync(string email, string password, string firstName, string lastName, CancellationToken ct)
        {
            var existing = await userManager.FindByEmailAsync(email);
            if (existing is not null)
                throw new InvalidOperationException("Email is already registered.");

            var user = new AppUser
            {
                Email = email,
                UserName = email,
                FirstName = firstName,
                LastName = lastName
            };

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(" | ", result.Errors.Select(e => e.Description)));

            var (token, expiresIn) = await jwt.CreateTokenAsync(user);
            return new AuthResultDto(token, expiresIn,user.FirstName, user.LastName);
        }

        public async Task<AuthResultDto> LoginAsync(string email, string password, CancellationToken ct)
        {
            var user = await userManager.FindByEmailAsync(email) ?? throw new UnauthorizedAccessException("Invalid credentials.");

            var result = await signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);
            if (!result.Succeeded)
                throw new UnauthorizedAccessException("Invalid credentials.");

            var (token, expiresIn) = await jwt.CreateTokenAsync(user);
            return new AuthResultDto(token, expiresIn, user.FirstName, user.LastName);
        }
    }
}
