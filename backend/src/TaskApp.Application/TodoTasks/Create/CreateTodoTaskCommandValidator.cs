using FluentValidation;

namespace TaskApp.Application.TodoTasks.Create
{
    public sealed class CreateTodoTaskCommandValidator : AbstractValidator<CreateTodoTaskCommand>
    {
        public CreateTodoTaskCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required.")
                .MaximumLength(200)
                .WithMessage("Title must not exceed 200 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(2000)
                .WithMessage("Description must not exceed 2000 characters.");
        }
    }
}
