namespace TaskApp.Application.Abstractions.Messaging
{
    public interface ICommandDispatcher
    {
        Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken ct = default);
    }
}
