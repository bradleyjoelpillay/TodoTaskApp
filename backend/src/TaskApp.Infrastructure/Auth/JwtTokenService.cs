using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskApp.Application.Abstractions.Auth;
using TaskApp.Domain.Entities;

namespace TaskApp.Infrastructure.Auth
{
    public sealed class JwtTokenService(IConfiguration config, UserManager<AppUser> userManager) : IJwtTokenService
    {
        public async Task<(string token, int expiresInSeconds)> CreateTokenAsync(AppUser user)
        {
            var jwt = config.GetSection("Jwt");
            var issuer = jwt["Issuer"]!;
            var audience = jwt["Audience"]!;
            var key = jwt["Key"]!;
            var expiryMinutes = int.Parse(jwt["ExpiryMinutes"] ?? "60");

            var roles = await userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Name, user.UserName ?? user.Email ?? user.Id),
                new(ClaimTypes.GivenName, $"{user.FirstName} {user.LastName}")
            };

            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.Now.AddMinutes(expiryMinutes);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return (tokenString, (int)TimeSpan.FromMinutes(expiryMinutes).TotalSeconds);
        }
    }
}
