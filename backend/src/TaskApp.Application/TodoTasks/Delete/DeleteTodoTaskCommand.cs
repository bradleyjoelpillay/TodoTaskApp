using TaskApp.Application.Abstractions.Messaging;

namespace TaskApp.Application.TodoTasks.Delete
{
    public record DeleteTodoTaskCommand(Guid Id) : ICommand<bool>;
}
