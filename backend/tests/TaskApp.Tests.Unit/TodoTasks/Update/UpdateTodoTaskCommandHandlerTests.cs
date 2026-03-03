using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TaskApp.Application.Abstractions.Persistence;
using TaskApp.Application.TodoTasks.Update;
using TaskApp.Domain.Entities;
using TaskApp.Domain.Enums;
using TaskApp.Infrastructure.Persistence;

namespace TaskApp.Tests.Unit.TodoTasks.Update
{
    public class UpdateTodoTaskCommandHandlerTests
    {
        [Fact]
        public async Task Handle_Should_Throw_ArgumentException_When_Id_Empty()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var ctx = new AppDbContext(options);
            var handler = new UpdateTodoTaskCommandHandler(ctx);

            var act = () => handler.Handle(
                new UpdateTodoTaskCommand(Guid.Empty, "New Task", "Some Description"),
                CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*cannot be empty*");
        }

        [Fact]
        public async Task Handle_Should_Throw_KeyNotFoundException_When_Task_Not_Found()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var ctx = new AppDbContext(options);
            var handler = new UpdateTodoTaskCommandHandler(ctx);

            var missingId = Guid.NewGuid();

            var act = () => handler.Handle(
                new UpdateTodoTaskCommand(missingId, "New Task", "Some Description"),
                CancellationToken.None);

            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage($"*{missingId}*was not found*");
        }

        [Fact]
        public async Task Handle_Should_Update_Task_And_Return_Dto_When_Saved()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var taskId = Guid.NewGuid();
            var now = DateTime.UtcNow;

            await using (var seed = new AppDbContext(options))
            {
                seed.Tasks.Add(new TodoTask
                {
                    Id = taskId,
                    Title = "Old",
                    Description = "OldDesc",
                    Status = StatusEnum.Todo,
                    CreatedAtUtc = now.AddMinutes(-10),
                    ModifiedAtUtc = now.AddMinutes(-10),
                    UserId = Guid.NewGuid().ToString(),
                });

                await seed.SaveChangesAsync();
            }

            await using var ctx = new AppDbContext(options);
            var handler = new UpdateTodoTaskCommandHandler(ctx);

            var before = DateTime.UtcNow;

            // Act
            var dto = await handler.Handle(
                new UpdateTodoTaskCommand(taskId, "New Title", "New Desc"),
                CancellationToken.None);

            // Assert DTO
            dto.Id.Should().Be(taskId);
            dto.Title.Should().Be("New Title");
            dto.Description.Should().Be("New Desc");

            // Assert persisted entity
            var updated = await ctx.Tasks.FindAsync(taskId);
            updated.Should().NotBeNull();
            updated!.Title.Should().Be("New Title");
            updated.Description.Should().Be("New Desc");
            updated.ModifiedAtUtc.Should().BeOnOrAfter(before);
        }

        [Fact]
        public async Task Handle_Should_Throw_InvalidOperationException_When_SaveChanges_Returns_Zero()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var taskId = Guid.NewGuid();

            await using (var seed = new AppDbContext(options))
            {
                seed.Tasks.Add(new TodoTask
                {
                    Id = taskId,
                    Title = "Old",
                    Description = "OldDesc",
                    Status = StatusEnum.Todo,
                    CreatedAtUtc = DateTime.UtcNow.AddMinutes(-10),
                    ModifiedAtUtc = DateTime.UtcNow.AddMinutes(-10),
                    UserId = Guid.NewGuid().ToString(),
                });

                await seed.SaveChangesAsync();
            }

            // Use a real EF context for FindAsync, but wrap SaveChangesAsync to return 0
            await using var inner = new AppDbContext(options);
            IAppDbContext ctx = new AppDbContextSaveZero(inner);

            var handler = new UpdateTodoTaskCommandHandler(ctx);

            // Act
            var act = () => handler.Handle(
                new UpdateTodoTaskCommand(taskId, "New Task", "Some Description"),
                CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*No changes were saved*");
        }

        // --- wrapper test double: SaveChangesAsync returns 0 ---
        private sealed class AppDbContextSaveZero(AppDbContext inner) : IAppDbContext
        {
            public DbSet<TodoTask> Tasks => inner.Tasks;

            public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
                => Task.FromResult(0);
        }

    }
}
