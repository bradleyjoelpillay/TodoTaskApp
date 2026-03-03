using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using TaskApp.Application.Abstractions.Persistence;
using TaskApp.Application.Abstractions.Utils;
using TaskApp.Application.TodoTasks.Create;
using TaskApp.Domain.Entities;
using TaskApp.Domain.Enums;

namespace TaskApp.Tests.Unit.TodoTasks.Create
{
    public sealed class CreateTodoTaskCommandHandlerTests
    {
        [Fact]
        public async Task Handle_Should_Add_Task_Save_And_Return_Dto()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var currentUser = new Mock<ICurrentUser>();
            currentUser.SetupGet(x => x.UserId).Returns(userId.ToString());

            TodoTask? addedEntity = null;

            var tasksSet = new Mock<DbSet<TodoTask>>();
            tasksSet
                .Setup(s => s.Add(It.IsAny<TodoTask>()))
                .Callback<TodoTask>(t => addedEntity = t);

            var db = new Mock<IAppDbContext>();
            db.SetupGet(x => x.Tasks).Returns(tasksSet.Object);
            db.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var handler = new CreateTodoTaskCommandHandler(db.Object, currentUser.Object);

            var command = new CreateTodoTaskCommand(
                Title: "Buy milk",
                Description: "2 liters"
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert - Db interactions
            tasksSet.Verify(s => s.Add(It.IsAny<TodoTask>()), Times.Once);
            db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            // Assert - entity created correctly
            addedEntity.Should().NotBeNull();
            addedEntity!.Id.Should().NotBeEmpty();
            addedEntity.Title.Should().Be("Buy milk");
            addedEntity.Description.Should().Be("2 liters");
            addedEntity.Status.Should().Be(StatusEnum.Todo);
            addedEntity.UserId.Should().Be(userId.ToString());
            addedEntity.CreatedAtUtc.Should().BeCloseTo(DateTime.UtcNow, precision: TimeSpan.FromSeconds(2));
            addedEntity.ModifiedAtUtc.Should().BeCloseTo(DateTime.UtcNow, precision: TimeSpan.FromSeconds(2));

            // Assert - DTO returned matches mapping
            result.Should().NotBeNull();
            result.Id.Should().Be(addedEntity.Id);
            result.Title.Should().Be(addedEntity.Title);
            result.Description.Should().Be(addedEntity.Description);
            result.Status.Should().Be(addedEntity.Status.ToString());
        }

        [Fact]
        public async Task Handle_Should_Use_CurrentUser_UserId()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var currentUser = new Mock<ICurrentUser>();
            currentUser.SetupGet(x => x.UserId).Returns(userId.ToString());

            TodoTask? addedEntity = null;

            var tasksSet = new Mock<DbSet<TodoTask>>();
            tasksSet
                .Setup(s => s.Add(It.IsAny<TodoTask>()))
                .Callback<TodoTask>(t => addedEntity = t);

            var db = new Mock<IAppDbContext>();
            db.SetupGet(x => x.Tasks).Returns(tasksSet.Object);
            db.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var handler = new CreateTodoTaskCommandHandler(db.Object, currentUser.Object);

            // Act
            await handler.Handle(new CreateTodoTaskCommand("Task", null), CancellationToken.None);

            // Assert
            addedEntity.Should().NotBeNull();
            addedEntity!.UserId.Should().Be(userId.ToString());
        }
    }
}
