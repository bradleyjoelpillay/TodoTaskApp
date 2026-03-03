using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TaskApp.Application.Abstractions.Utils;

namespace TaskApp.Infrastructure.Auth
{
    public class CurrentUser(IHttpContextAccessor accessor) : ICurrentUser
    {
        public string? UserId =>
            accessor.HttpContext?
                .User?
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public bool IsAuthenticated =>
            accessor.HttpContext?
                .User?
                .Identity?.IsAuthenticated ?? false;
    }
}
