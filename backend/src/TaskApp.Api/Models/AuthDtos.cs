namespace TaskApp.Api.Models
{
    // Request Dtos
    public sealed record RegisterRequest(string Email, string Password, string FirstName, string LastName);
    public sealed record LoginRequest(string Email, string Password);

    // Response Dtos
    public sealed record AuthResponse(string AccessToken, int ExpiresIn, string FullName);
}
