using TaskApp.Application.Abstractions.Messaging;
using TaskApp.Application.Abstractions.Persistence;

namespace TaskApp.Application.TodoTasks.Update
{
    public class UpdateTodoTaskCommandHandler(IAppDbContext dbContext) : ICommandHandler<UpdateTodoTaskCommand, TodoTaskDto>
    {
        public async Task<TodoTaskDto> Handle(UpdateTodoTaskCommand command, CancellationToken cancellationToken)
        {

            if (command.Id == Guid.Empty)
                throw new ArgumentException("Task ID cannot be empty.", nameof(command.Id));

            var task = await dbContext.Tasks.FindAsync([command.Id], cancellationToken) ?? throw new KeyNotFoundException($"Task with id '{command.Id}' was not found.");

            task.Title = command.Title;
            task.Description = command.Description;
            task.ModifiedAtUtc = DateTime.UtcNow;

            var saved = await dbContext.SaveChangesAsync(cancellationToken) > 0;

            if (saved) return task.ToDto();

            throw new InvalidOperationException("Update failed. No changes were saved.");

        }
    }
}
