using TaskApp.Application.Abstractions.Messaging;
using TaskApp.Domain.Enums;

namespace TaskApp.Application.TodoTasks.Update
{
    public record UpdateTodoTaskCommand(Guid Id, string Title, string? Description) : ICommand<TodoTaskDto>;
}
