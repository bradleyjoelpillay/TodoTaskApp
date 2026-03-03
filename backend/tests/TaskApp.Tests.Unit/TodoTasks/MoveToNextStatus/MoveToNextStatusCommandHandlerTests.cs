using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TaskApp.Application.TodoTasks.MoveToNextStatus;
using TaskApp.Domain.Entities;
using TaskApp.Domain.Enums;
using TaskApp.Infrastructure.Persistence;

namespace TaskApp.Tests.Unit.TodoTasks.MoveToNextStatus
{
    public class MoveToNextStatusCommandHandlerTests
    {
        [Fact]
        public async Task Handle_Should_Throw_ArgumentException_When_Id_Empty()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var ctx = new AppDbContext(options);
            var handler = new MoveToNextStatusCommandHandler(ctx);

            // Act
            var act = () => handler.Handle(new MoveToNextStatusCommand(Guid.Empty), CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*cannot be empty*");
        }

        [Fact]
        public async Task Handle_Should_Throw_KeyNotFoundException_When_Task_Not_Found()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var ctx = new AppDbContext(options);
            var handler = new MoveToNextStatusCommandHandler(ctx);

            var missingId = Guid.NewGuid();

            // Act
            var act = () => handler.Handle(new MoveToNextStatusCommand(missingId), CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage($"*{missingId}*was not found*");
        }

        [Theory]
        [InlineData(StatusEnum.Todo, StatusEnum.InProgress)]
        [InlineData(StatusEnum.InProgress, StatusEnum.Done)]
        [InlineData(StatusEnum.Done, StatusEnum.Done)]
        public async Task Handle_Should_Move_Status_To_Next_Value(StatusEnum start, StatusEnum expected)
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
                    Title = "T",
                    Description = "D",
                    Status = start,
                    CreatedAtUtc = DateTime.UtcNow.AddMinutes(-5),
                    ModifiedAtUtc = DateTime.UtcNow.AddMinutes(-1),
                    UserId = Guid.NewGuid().ToString()
                });

                await seed.SaveChangesAsync();
            }

            await using var ctx = new AppDbContext(options);
            var handler = new MoveToNextStatusCommandHandler(ctx);

            // Act
            var dto = await handler.Handle(new MoveToNextStatusCommand(taskId), CancellationToken.None);

            // Assert - returned DTO reflects new status
            dto.Status.Should().Be(expected.ToString());

            // Assert - persisted entity updated too
            var updated = await ctx.Tasks.FindAsync(taskId);
            updated.Should().NotBeNull();
            updated!.Status.Should().Be(expected);
        }
    }
}
