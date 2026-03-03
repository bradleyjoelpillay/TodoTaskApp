using Microsoft.EntityFrameworkCore;
using TaskApp.Domain.Entities;

namespace TaskApp.Application.Abstractions.Persistence
{
    public interface IAppDbContext
    {
        DbSet<TodoTask> Tasks { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
