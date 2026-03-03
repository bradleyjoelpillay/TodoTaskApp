using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskApp.Application.Abstractions.Auth;
using TaskApp.Application.Abstractions.Messaging;
using TaskApp.Application.Abstractions.Persistence;
using TaskApp.Application.Abstractions.Utils;
using TaskApp.Domain.Entities;
using TaskApp.Infrastructure.Auth;
using TaskApp.Infrastructure.Messaging;
using TaskApp.Infrastructure.Persistence;

namespace TaskApp.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            var connString = config.GetConnectionString("DefaultConnection")
                     ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(connString));

            services.AddIdentityCore<AppUser>(opt =>
            {
                opt.Password.RequiredLength = 8;
                opt.Password.RequireNonAlphanumeric = false;
                opt.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddSignInManager();

            services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICommandDispatcher, CommandDispatcher>();
            services.AddScoped<IQueryDispatcher, QueryDispatcher>();
            services.AddScoped<ICurrentUser, CurrentUser>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            return services;
        }
    }
}
