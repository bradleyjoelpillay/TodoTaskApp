using TaskApp.Application.Abstractions.Messaging;

namespace TaskApp.Application.Users.Login
{
    public sealed record UserLoginCommand(string Email, string Password) : ICommand<AuthResultDto>;
}
