using FluentValidation;

namespace TaskApp.Application.TodoTasks.Delete
{
    public class DeleteTodoTaskCommandValidator : AbstractValidator<DeleteTodoTaskCommand>
    {
        public DeleteTodoTaskCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Task ID must not be empty.")
                .Must(id => Guid.TryParse(id.ToString(), out _)).WithMessage("Task ID must be a valid GUID.");
        }
    }
}
