using TaskApp.Application.Abstractions.Messaging;
using TaskApp.Application.Abstractions.Persistence;

namespace TaskApp.Application.TodoTasks.Delete
{
    public class DeleteTodoTaskCommandHandler(IAppDbContext dbContext) : ICommandHandler<DeleteTodoTaskCommand, bool>
    {
        public async Task<bool> Handle(DeleteTodoTaskCommand command, CancellationToken ct)
        {

            if (command.Id == Guid.Empty)
                throw new ArgumentException("Task ID cannot be empty.", nameof(command.Id));

            var task = await dbContext.Tasks.FindAsync([command.Id], ct) ?? throw new KeyNotFoundException($"Task with id '{command.Id}' was not found.");

            dbContext.Tasks.Remove(task);
            
            return await dbContext.SaveChangesAsync(ct) > 0;
        }
    }
}
