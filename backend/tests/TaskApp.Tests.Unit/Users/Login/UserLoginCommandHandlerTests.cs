using FluentAssertions;
using Moq;
using TaskApp.Application.Abstractions.Auth;
using TaskApp.Application.Users;
using TaskApp.Application.Users.Login;

namespace TaskApp.Tests.Unit.Users.Login
{
    public class UserLoginCommandHandlerTests
    {
        [Fact]
        public async Task Handle_Should_Call_LoginAsync_And_Return_Result()
        {
            // Arrange
            var auth = new Mock<IAuthService>(MockBehavior.Strict);

            var expected = new AuthResultDto(
                AccessToken: "token",
                ExpiresIn: 3600,
                FirstName: "Brad",
                LastName: "Pillay");

            auth.Setup(x => x.LoginAsync("a@b.com", "pass123", It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var handler = new UserLoginCommandHandler(auth.Object);

            var ct = new CancellationTokenSource().Token;
            var command = new UserLoginCommand("a@b.com", "pass123");

            // Act
            var result = await handler.Handle(command, ct);

            // Assert
            result.Should().Be(expected);

            auth.Verify(x => x.LoginAsync("a@b.com", "pass123", ct), Times.Once);
            auth.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_Should_Propagate_Exception_From_AuthService()
        {
            // Arrange
            var auth = new Mock<IAuthService>(MockBehavior.Strict);

            auth.Setup(x => x.LoginAsync("a@b.com", "pass123", It.IsAny<CancellationToken>()))
                .ThrowsAsync(new UnauthorizedAccessException("Invalid credentials."));

            var handler = new UserLoginCommandHandler(auth.Object);

            var command = new UserLoginCommand("a@b.com", "pass123");

            // Act
            var act = () => handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("*Invalid credentials*");

            auth.Verify(x => x.LoginAsync("a@b.com", "pass123", It.IsAny<CancellationToken>()), Times.Once);
            auth.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_Should_Pass_CancellationToken_To_AuthService()
        {
            // Arrange
            var auth = new Mock<IAuthService>(MockBehavior.Strict);

            var expected = new AuthResultDto("token", 3600, "A", "B");

            var cts = new CancellationTokenSource();
            var ct = cts.Token;

            auth.Setup(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>(), ct))
                .ReturnsAsync(expected);

            var handler = new UserLoginCommandHandler(auth.Object);

            // Act
            var result = await handler.Handle(new UserLoginCommand("x@y.com", "pw"), ct);

            // Assert
            result.Should().Be(expected);

            auth.Verify(x => x.LoginAsync("x@y.com", "pw", ct), Times.Once);
            auth.VerifyNoOtherCalls();
        }

    }
}
