using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using TaskApp.Application.Abstractions.Utils;
using TaskApp.Application.TodoTasks.List;
using TaskApp.Domain.Entities;
using TaskApp.Domain.Enums;
using TaskApp.Infrastructure.Persistence;

namespace TaskApp.Tests.Unit.TodoTasks.List
{
    public class GetTodoTasksQueryHandlerTests
    {
        [Fact]
        public async Task Handle_Should_Return_PagedResult_With_Correct_Items_Order_And_Metadata()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var currentUser = new Mock<ICurrentUser>();
            currentUser.SetupGet(x => x.UserId).Returns(userId.ToString());

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(userId.ToString())
                .Options;

            var baseTime = DateTime.UtcNow;

            var tasks = new List<TodoTask>
            {
                new() { Id = Guid.NewGuid(), Title = "T1", Description = null, Status = StatusEnum.Todo, CreatedAtUtc = baseTime.AddMinutes(-1),  ModifiedAtUtc = baseTime, UserId = userId.ToString() },
                new() { Id = Guid.NewGuid(), Title = "T2", Description = "D2", Status = StatusEnum.Done, CreatedAtUtc = baseTime.AddMinutes(-2),  ModifiedAtUtc = baseTime, UserId = userId.ToString() },
                new() { Id = Guid.NewGuid(), Title = "T3", Description = "D3", Status = StatusEnum.InProgress, CreatedAtUtc = baseTime.AddMinutes(-3), ModifiedAtUtc = baseTime, UserId = userId.ToString() },
                new() { Id = Guid.NewGuid(), Title = "T4", Description = null, Status = StatusEnum.Todo, CreatedAtUtc = baseTime.AddMinutes(-4),  ModifiedAtUtc = baseTime, UserId = userId.ToString() },
                new() { Id = Guid.NewGuid(), Title = "T5", Description = "D5", Status = StatusEnum.Done, CreatedAtUtc = baseTime.AddMinutes(-5),  ModifiedAtUtc = baseTime, UserId = userId.ToString() },
            };

            await using (var seed = new AppDbContext(options))
            {
                seed.Tasks.AddRange(tasks);
                await seed.SaveChangesAsync();
            }

            await using var ctx = new AppDbContext(options);
            var handler = new GetTodoTasksQueryHandler(ctx, currentUser.Object);

            // Page 1, page size 2 => newest 2 tasks
            var query = new GetTodoTasksQuery(PageNumber: 1, PageSize: 2);

            // Act
            var paged = await handler.Handle(query, CancellationToken.None);

            // Assert
            paged.Should().NotBeNull();

            paged.TotalCount.Should().Be(5);
            paged.TotalPages.Should().Be(3); 

            var items = paged.Items; 
            items.Should().HaveCount(2);

            items[0].Title.Should().Be("T1");
            items[1].Title.Should().Be("T2");

            items[0].Description.Should().Be(string.Empty); 
            items[0].Status.Should().Be(StatusEnum.Todo.ToString());

            items[1].Description.Should().Be("D2");
            items[1].Status.Should().Be(StatusEnum.Done.ToString());

            items[0].UserId.Should().Be(userId.ToString());
            items[1].UserId.Should().Be(userId.ToString());
        }

        [Fact]
        public async Task Handle_Should_Return_Second_Page_Correctly()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var currentUser = new Mock<ICurrentUser>();
            currentUser.SetupGet(x => x.UserId).Returns(userId.ToString());

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var baseTime = DateTime.UtcNow;

            var tasks = Enumerable.Range(1, 5)
                .Select(i => new TodoTask
                {
                    Id = Guid.NewGuid(),
                    Title = $"T{i}",
                    Description = i % 2 == 0 ? $"D{i}" : null,
                    Status = StatusEnum.Todo,
                    CreatedAtUtc = baseTime.AddMinutes(-i), 
                    ModifiedAtUtc = baseTime,
                    UserId = userId.ToString()
                })
                .ToList();

            await using (var seed = new AppDbContext(options))
            {
                seed.Tasks.AddRange(tasks);
                await seed.SaveChangesAsync();
            }

            await using var ctx = new AppDbContext(options);
            var handler = new GetTodoTasksQueryHandler(ctx, currentUser.Object);

            // Page 2, size 2 => items 3rd and 4th newest => T3, T4
            var query = new GetTodoTasksQuery(PageNumber: 2, PageSize: 2);

            // Act
            var paged = await handler.Handle(query, CancellationToken.None);

            // Assert
            var items = paged.Items; // <-- adjust if different
            items.Should().HaveCount(2);

            items[0].Title.Should().Be("T3");
            items[1].Title.Should().Be("T4");
        }

    }
}
