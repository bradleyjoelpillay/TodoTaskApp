using FluentAssertions;
using Moq;
using TaskApp.Application.Abstractions.Auth;
using TaskApp.Application.Users;
using TaskApp.Application.Users.Register;

namespace TaskApp.Tests.Unit.Users.Register
{
    public class RegisterUserCommandHandlerTests
    {
        [Fact]
        public async Task Handle_Should_Call_RegisterAsync_And_Return_Result()
        {
            // Arrange
            var auth = new Mock<IAuthService>(MockBehavior.Strict);

            var expected = new AuthResultDto(
                AccessToken: "token123",
                ExpiresIn: 3600,
                FirstName: "Brad",
                LastName: "Pillay");

            auth.Setup(x => x.RegisterAsync(
                    "test@mail.com",
                    "Pass123!",
                    "Brad",
                    "Pillay",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var handler = new RegisterUserCommandHandler(auth.Object);

            var ct = new CancellationTokenSource().Token;
            var command = new RegisterUserCommand(
                Email: "test@mail.com",
                Password: "Pass123!",
                FirstName: "Brad",
                LastName: "Pillay");

            // Act
            var result = await handler.Handle(command, ct);

            // Assert
            result.Should().Be(expected);

            auth.Verify(x => x.RegisterAsync(
                "test@mail.com",
                "Pass123!",
                "Brad",
                "Pillay",
                ct), Times.Once);

            auth.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_Should_Propagate_Exception_From_AuthService()
        {
            // Arrange
            var auth = new Mock<IAuthService>(MockBehavior.Strict);

            auth.Setup(x => x.RegisterAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Email already exists."));

            var handler = new RegisterUserCommandHandler(auth.Object);

            var command = new RegisterUserCommand(
                Email: "test@mail.com",
                Password: "Pass123!",
                FirstName: "Brad",
                LastName: "Pillay");

            // Act
            var act = () => handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*Email already exists*");

            auth.Verify(x => x.RegisterAsync(
                "test@mail.com",
                "Pass123!",
                "Brad",
                "Pillay",
                It.IsAny<CancellationToken>()), Times.Once);

            auth.VerifyNoOtherCalls();
        }


    }
}
