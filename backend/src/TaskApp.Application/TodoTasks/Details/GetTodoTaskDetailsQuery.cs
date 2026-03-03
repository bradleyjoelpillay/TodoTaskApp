using TaskApp.Application.Abstractions.Messaging;

namespace TaskApp.Application.TodoTasks.Details
{
    public sealed record GetTodoTaskDetailsQuery(Guid Id) : IQuery<TodoTaskDto>;
}
