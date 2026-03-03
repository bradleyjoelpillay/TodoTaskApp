using TaskApp.Application.Abstractions.Auth;
using TaskApp.Application.Abstractions.Messaging;

namespace TaskApp.Application.Users.Login
{
    public sealed class UserLoginCommandHandler(IAuthService auth) : ICommandHandler<UserLoginCommand, AuthResultDto>
    {
        public Task<AuthResultDto> Handle(UserLoginCommand command, CancellationToken ct)
            => auth.LoginAsync(command.Email, command.Password, ct);
    }
}
