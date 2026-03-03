using FluentValidation;

namespace TaskApp.Application.Users.Register
{
    public sealed class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.");
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.");
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.");
            RuleFor(x => x)
                .Must(x => x.Password != null && !x.Password.Contains(" "))
                .WithMessage("Password must not contain spaces.");
        }
    }
}
