using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using TaskApp.Application.Abstractions.Messaging;

namespace TaskApp.Infrastructure.Messaging
{
    public sealed class CommandDispatcher(IServiceProvider serviceProvider) : ICommandDispatcher
    {
        public async Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
        {
            if (command is null) throw new ArgumentNullException(nameof(command));

            await ValidateAsync(command, cancellationToken);

            // Resolve ICommandHandler<TCommand, TResponse>
            var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResponse));
            var handler = serviceProvider.GetRequiredService(handlerType);

            var handleMethod = handlerType.GetMethod(nameof(ICommandHandler<ICommand<TResponse>, TResponse>.Handle));
            if (handleMethod is null)
                throw new InvalidOperationException($"Handler '{handlerType.FullName}' does not contain Handle().");

            var resultObj = handleMethod.Invoke(handler, [command, cancellationToken]);

            if (resultObj is Task<TResponse> task)
                return await task;

            throw new InvalidOperationException($"Handler '{handlerType.FullName}' returned an invalid task type.");
        }

        private async Task ValidateAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken)
        {
            var commandType = command.GetType();

            // Resolve all IValidator<TCommand>
            var validatorType = typeof(IValidator<>).MakeGenericType(commandType);
            var validators = serviceProvider.GetServices(validatorType).Cast<IValidator>().ToArray();

            if (validators.Length == 0)
                return;

            // Build ValidationContext<TCommand> at runtime
            var contextType = typeof(ValidationContext<>).MakeGenericType(commandType);
            var context = (IValidationContext)Activator.CreateInstance(contextType, command)!;

            var failures = new List<ValidationFailure>();

            foreach (var validator in validators)
            {
                var result = await validator.ValidateAsync(context, cancellationToken);
                if (!result.IsValid)
                    failures.AddRange(result.Errors);
            }

            if (failures.Count > 0)
                throw new ValidationException(failures);
        }
    }
}
