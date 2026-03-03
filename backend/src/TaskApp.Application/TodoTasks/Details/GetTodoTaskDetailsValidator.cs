using FluentValidation;

namespace TaskApp.Application.TodoTasks.Details
{
    public sealed class GetTodoTaskDetailsValidator : AbstractValidator<GetTodoTaskDetailsQuery>
    {
        public GetTodoTaskDetailsValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required.")
                .Must(id => Guid.TryParse(id.ToString(), out _)).WithMessage("Id must be a valid GUID.");
        }
    }
}
