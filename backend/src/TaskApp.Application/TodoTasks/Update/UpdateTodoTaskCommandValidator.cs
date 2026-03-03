using FluentValidation;

namespace TaskApp.Application.TodoTasks.Update
{
    public sealed class UpdateTodoTaskCommandValidator : AbstractValidator<UpdateTodoTaskCommand>
    {
        public UpdateTodoTaskCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Task ID cannot be empty.");
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");
            RuleFor(x => x.Description)
                .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters.");
            
        }
    }
}
