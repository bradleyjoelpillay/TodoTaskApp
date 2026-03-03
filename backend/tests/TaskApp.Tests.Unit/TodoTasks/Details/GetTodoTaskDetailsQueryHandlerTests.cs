using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TaskApp.Application.TodoTasks.Details;
using TaskApp.Domain.Entities;
using TaskApp.Domain.Enums;
using TaskApp.Domain.Exceptions;
using TaskApp.Infrastructure.Persistence;

namespace TaskApp.Tests.Unit.TodoTasks.Details
{
    public class GetTodoTaskDetailsQueryHandlerTests
    {
        [Fact]
        public async Task Handle_Should_Return_Dto_When_Task_Exists()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            await using (var seed = new AppDbContext(options))
            {
                seed.Tasks.Add(new TodoTask
                {
                    Id = taskId,
                    Title = "Read book",
                    Description = "Chapters 1 to 3",
                    Status = StatusEnum.Todo,
                    CreatedAtUtc = DateTime.UtcNow.AddMinutes(-10),
                    ModifiedAtUtc = DateTime.UtcNow.AddMinutes(-5),
                    UserId = userId.ToString()
                });

                await seed.SaveChangesAsync();
            }

            await using var ctx = new AppDbContext(options);
            var handler = new GetTodoTaskDetailsQueryHandler(ctx);

            var query = new GetTodoTaskDetailsQuery(taskId);

            // Act
            var dto = await handler.Handle(query, CancellationToken.None);

            // Assert
            dto.Should().NotBeNull();
            dto.Id.Should().Be(taskId);
            dto.Title.Should().Be("Read book");
            dto.Description.Should().Be("Chapters 1 to 3");
            dto.Status.Should().Be(StatusEnum.Todo.ToString());
        }

        [Fact]
        public async Task Handle_Should_Throw_NotFoundException_When_Task_Does_Not_Exist()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var ctx = new AppDbContext(options);
            var handler = new GetTodoTaskDetailsQueryHandler(ctx);

            var missingId = Guid.NewGuid();
            var query = new GetTodoTaskDetailsQuery(missingId);

            // Act
            var act = () => handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"*{missingId}*not found*");
        }

    }
}
