using TaskApp.Domain.Entities;

namespace TaskApp.Application.TodoTasks
{
    public static class TodoTaskMappings
    {
        public static TodoTaskDto ToDto(this TodoTask t) =>
            new(t.Id, t.Title, t.Description, t.Status.ToString(), t.CreatedAtUtc, t.ModifiedAtUtc);
    }
}
