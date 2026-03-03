using TaskApp.Application.Abstractions.Messaging;
using TaskApp.Application.Common;

namespace TaskApp.Application.TodoTasks.List
{
    public sealed record GetTodoTasksQuery(int PageNumber, int PageSize) : IQuery<PagedResult<TodoTaskResponse>>;
}
