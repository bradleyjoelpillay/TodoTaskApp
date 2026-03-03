using TaskApp.Domain.Enums;

namespace TaskApp.Application.TodoTasks
{
    public record TodoTaskDto(Guid Id, string Title, string? Description, string Status, DateTime CreatedAtUtc, DateTime UpdatedAtUtc);
    public class TodoTaskResponse
    {
        public Guid Id { get; init; }
        public string Title { get; init; }
        public string Description { get; init; }
        public string Status { get; init; }
        public DateTime CreatedAtUtc { get; init; }
        public string UserId { get; init; }
    }
    public record CreateTaskRequest(string Title, string? Description);
    public record UpdateTaskRequest(string Title, string? Description, StatusEnum Status);
}
