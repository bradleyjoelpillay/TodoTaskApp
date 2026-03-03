using Microsoft.AspNetCore.Mvc;
using TaskApp.Api.Models;
using TaskApp.Application.Abstractions.Messaging;
using TaskApp.Application.Users.Login;
using TaskApp.Application.Users.Register;

namespace TaskApp.Api.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public sealed class AuthController(ICommandDispatcher dispatcher) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req, CancellationToken ct)
        {
            var result = await dispatcher.Send(new RegisterUserCommand(req.Email, req.Password, req.FirstName, req.LastName), ct);

            return Ok(new AuthResponse(result.AccessToken, result.ExpiresIn, $"{result.FirstName} {result.LastName}"));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req, CancellationToken ct)
        {
            var result = await dispatcher.Send(new UserLoginCommand(req.Email, req.Password), ct);
            return Ok(new AuthResponse(result.AccessToken, result.ExpiresIn, $"{result.FirstName} {result.LastName}"));
        }
    }
}
