using TaskApp.Application.Abstractions.Messaging;
using TaskApp.Application.Abstractions.Persistence;
using TaskApp.Application.Abstractions.Utils;
using TaskApp.Domain.Entities;
using TaskApp.Domain.Enums;

namespace TaskApp.Application.TodoTasks.Create
{
    public class CreateTodoTaskCommandHandler(IAppDbContext dbContext, ICurrentUser currentUser) : ICommandHandler<CreateTodoTaskCommand, TodoTaskDto>
    {
        public async Task<TodoTaskDto> Handle(CreateTodoTaskCommand command, CancellationToken ct)
        {

            var task = new TodoTask
            {
                Id = Guid.NewGuid(),
                Title = command.Title,
                Description = command.Description,
                Status = StatusEnum.Todo,
                CreatedAtUtc = DateTime.UtcNow,
                ModifiedAtUtc = DateTime.UtcNow,
                UserId = currentUser.UserId
            };
            
            dbContext.Tasks.Add(task);
            
            await dbContext.SaveChangesAsync(ct);

            return task.ToDto();
        }
    }
}
