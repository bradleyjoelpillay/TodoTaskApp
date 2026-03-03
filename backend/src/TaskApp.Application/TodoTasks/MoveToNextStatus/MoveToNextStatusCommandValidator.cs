using FluentValidation;

namespace TaskApp.Application.TodoTasks.MoveToNextStatus
{
    public class MoveToNextStatusCommandValidator : AbstractValidator<MoveToNextStatusCommand>
    {
        public MoveToNextStatusCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Task ID cannot be empty.");
        }
    }
}
