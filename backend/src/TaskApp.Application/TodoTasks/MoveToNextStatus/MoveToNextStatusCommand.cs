using TaskApp.Application.Abstractions.Messaging;

namespace TaskApp.Application.TodoTasks.MoveToNextStatus
{
    public record MoveToNextStatusCommand(Guid Id) : ICommand<TodoTaskDto>;
}
