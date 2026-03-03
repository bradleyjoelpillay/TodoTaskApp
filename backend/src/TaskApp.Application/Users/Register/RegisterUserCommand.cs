using TaskApp.Application.Abstractions.Messaging;

namespace TaskApp.Application.Users.Register
{
    public sealed record RegisterUserCommand(string Email, string Password, string FirstName, string LastName) : ICommand<AuthResultDto>;
}
