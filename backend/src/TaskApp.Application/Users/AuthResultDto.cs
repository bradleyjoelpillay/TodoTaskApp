namespace TaskApp.Application.Users
{
    public sealed record AuthResultDto(string AccessToken, int ExpiresIn, string FirstName, string LastName);
}
