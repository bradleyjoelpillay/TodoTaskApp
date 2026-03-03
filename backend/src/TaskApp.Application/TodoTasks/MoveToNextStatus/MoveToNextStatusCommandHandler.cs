using TaskApp.Application.Abstractions.Messaging;
using TaskApp.Application.Abstractions.Persistence;
using TaskApp.Domain.Enums;

namespace TaskApp.Application.TodoTasks.MoveToNextStatus
{
    public sealed class MoveToNextStatusCommandHandler(IAppDbContext dbContext) : ICommandHandler<MoveToNextStatusCommand, TodoTaskDto>
    {
        public async Task<TodoTaskDto> Handle(MoveToNextStatusCommand command, CancellationToken cancellationToken)
        {
            if (command.Id == Guid.Empty)
                throw new ArgumentException("Task ID cannot be empty.", nameof(command.Id));

            var task = await dbContext.Tasks.FindAsync([command.Id], cancellationToken) ?? throw new KeyNotFoundException($"Task with id '{command.Id}' was not found.");

            task.Status = task.Status switch
            {
                StatusEnum.Todo => StatusEnum.InProgress,
                StatusEnum.InProgress => StatusEnum.Done,
                StatusEnum.Done => StatusEnum.Done,
                _ => StatusEnum.Done,
            };
            task.ModifiedAtUtc = DateTime.UtcNow;

            await dbContext.SaveChangesAsync(cancellationToken);

            return task.ToDto();
        }
    }
}
