using TaskApp.Application.Abstractions.Messaging;
using TaskApp.Application.Abstractions.Persistence;
using TaskApp.Application.Abstractions.Utils;
using TaskApp.Application.Common;

namespace TaskApp.Application.TodoTasks.List
{
    public class GetTodoTasksQueryHandler(IAppDbContext dbContext, ICurrentUser currentUser) : IQueryHandler<GetTodoTasksQuery, PagedResult<TodoTaskResponse>>
    {
        public async Task<PagedResult<TodoTaskResponse>> Handle(GetTodoTasksQuery query, CancellationToken ct)
        {
            var todoTasksQuery = dbContext.Tasks
                .Select(t => new TodoTaskResponse
                {
                    CreatedAtUtc = t.CreatedAtUtc,
                    Title = t.Title,
                    Status = t.Status.ToString(),
                    Description = t.Description ?? string.Empty,
                    Id = t.Id,
                    UserId = t.UserId
                })
                .Where(x => x.UserId == currentUser.UserId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .AsQueryable();
            var result = await PagedList<TodoTaskResponse>.CreateAsync(todoTasksQuery, query.PageNumber, query.PageSize);

            return new PagedResult<TodoTaskResponse>(result, query.PageNumber, query.PageSize, result.TotalCount, result.TotalPages);
        }
    }
}
