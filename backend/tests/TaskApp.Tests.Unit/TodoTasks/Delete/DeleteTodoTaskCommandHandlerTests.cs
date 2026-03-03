using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using TaskApp.Application.Abstractions.Persistence;
using TaskApp.Application.TodoTasks.Delete;
using TaskApp.Domain.Entities;
using TaskApp.Infrastructure.Persistence;

namespace TaskApp.Tests.Unit.TodoTasks.Delete
{
    public sealed class DeleteTodoTaskCommandHandlerTests
    {
        [Fact]
        public async Task Handle_Should_Remove_Task_And_Return_True_When_SaveChanges_Positive()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var taskId = Guid.NewGuid();

            await using (var seedCtx = new AppDbContext(options))
            {
                seedCtx.Tasks.Add(new TodoTask
                {
                    Id = taskId,
                    Title = "Task 01",
                    Description = "My first task to delete",
                    UserId = Guid.NewGuid().ToString(),
                });

                await seedCtx.SaveChangesAsync();
            }

            await using var ctx = new AppDbContext(options);

            var handler = new DeleteTodoTaskCommandHandler(ctx);

            // Act
            var result = await handler.Handle(new DeleteTodoTaskCommand(taskId), CancellationToken.None);

            // Assert
            result.Should().BeTrue();

            var deleted = await ctx.Tasks.FindAsync(taskId);
            deleted.Should().BeNull();
        }

        [Fact]
        public async Task Handle_Should_Throw_KeyNotFoundException_When_Task_Not_Found()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            await using var ctx = new AppDbContext(options);
            var handler = new DeleteTodoTaskCommandHandler(ctx);

            // Act
            var act = () => handler.Handle(new DeleteTodoTaskCommand(Guid.NewGuid()), CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("*was not found*");
        }

        [Fact]
        public async Task Handle_Should_Throw_ArgumentException_When_Id_Empty()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            await using var ctx = new AppDbContext(options);
            var handler = new DeleteTodoTaskCommandHandler(ctx);

            // Act
            var act = () => handler.Handle(new DeleteTodoTaskCommand(Guid.Empty), CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*cannot be empty*");
        }
        
    }
}
