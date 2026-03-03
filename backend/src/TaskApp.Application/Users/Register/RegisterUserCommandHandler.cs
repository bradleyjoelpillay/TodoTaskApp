using TaskApp.Application.Abstractions.Auth;
using TaskApp.Application.Abstractions.Messaging;

namespace TaskApp.Application.Users.Register
{
    public sealed class RegisterUserCommandHandler(IAuthService auth) : ICommandHandler<RegisterUserCommand, AuthResultDto>
    {
        public Task<AuthResultDto> Handle(RegisterUserCommand command, CancellationToken ct)
            => auth.RegisterAsync(command.Email, command.Password, command.FirstName, command.LastName, ct);
    }
}
