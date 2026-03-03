using TaskApp.Application.Abstractions.Messaging;

namespace TaskApp.Application.TodoTasks.Create
{
    public record CreateTodoTaskCommand(string Title, string? Description) : ICommand<TodoTaskDto>;
}
