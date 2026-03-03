namespace TaskApp.Application.Abstractions.Utils
{
    public interface ICurrentUser
    {
        string? UserId { get; }
        bool IsAuthenticated { get; }
    }
}
