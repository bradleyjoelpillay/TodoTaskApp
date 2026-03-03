using FluentValidation;

namespace TaskApp.Application.TodoTasks.List
{
    public sealed class GetTodoTasksQueryValidator : AbstractValidator<GetTodoTasksQuery>
    {
        public GetTodoTasksQueryValidator() { 
            RuleFor(x => x.PageNumber)
                .GreaterThan(0).WithMessage("Page number must be greater than 0.");

            RuleFor(x => x.PageSize)
                .GreaterThan(0).WithMessage("Page size must be greater than 0.");
        }
    }
}
