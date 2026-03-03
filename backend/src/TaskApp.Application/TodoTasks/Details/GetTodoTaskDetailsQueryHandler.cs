using Microsoft.EntityFrameworkCore;
using TaskApp.Application.Abstractions.Messaging;
using TaskApp.Application.Abstractions.Persistence;
using TaskApp.Domain.Exceptions;

namespace TaskApp.Application.TodoTasks.Details
{
    public class GetTodoTaskDetailsQueryHandler(IAppDbContext dbContext) : IQueryHandler<GetTodoTaskDetailsQuery, TodoTaskDto>
    {
        public async Task<TodoTaskDto> Handle(GetTodoTaskDetailsQuery query, CancellationToken ct)
        {
            var task = await dbContext.Tasks
                .AsNoTracking()
                .Where(t => t.Id == query.Id)
                .FirstOrDefaultAsync(ct);

            return task is null ? throw new NotFoundException($"Task {query.Id} not found.") : task.ToDto();
        }
    }
}
