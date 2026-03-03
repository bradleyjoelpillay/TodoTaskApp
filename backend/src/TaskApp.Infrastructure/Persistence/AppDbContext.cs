using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskApp.Application.Abstractions.Persistence;
using TaskApp.Domain.Entities;

namespace TaskApp.Infrastructure.Persistence
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<AppUser>(options), IAppDbContext
    {
        public DbSet<TodoTask> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            modelBuilder.Entity<TodoTask>(b =>
            {
                b.ToTable("TodoTasks");
                b.HasKey(x => x.Id);
                b.Property(x => x.Title).HasMaxLength(200).IsRequired();
                b.Property(x => x.Description).HasMaxLength(2000);
                b.Property(x => x.Status).HasConversion<string>().IsRequired();
                b.Property(x => x.CreatedAtUtc).IsRequired();
            });
        }
    }
}
