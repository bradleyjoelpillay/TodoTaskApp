using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using TaskApp.Application.Abstractions.Messaging;

namespace TaskApp.Infrastructure.Messaging
{
    public sealed class QueryDispatcher(IServiceProvider serviceProvider) : IQueryDispatcher
    {
        public async Task<TResponse> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
        {
            if (query is null) throw new ArgumentNullException(nameof(query));

            await ValidateAsync(query, cancellationToken);

            // Resolve IQueryHandler<TQuery, TResponse>
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResponse));
            var handler = serviceProvider.GetRequiredService(handlerType);

            var handleMethod = handlerType.GetMethod(nameof(IQueryHandler<IQuery<TResponse>, TResponse>.Handle)) ?? throw new InvalidOperationException($"Handler '{handlerType.FullName}' does not contain Handle().");
            var resultObj = handleMethod.Invoke(handler, [query, cancellationToken]);

            if (resultObj is Task<TResponse> task)
                return await task;

            throw new InvalidOperationException($"Handler '{handlerType.FullName}' returned an invalid task type.");
        }

        private async Task ValidateAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken)
        {
            var queryType = query.GetType();

            // Resolve all IValidator<TQuery>
            var validatorType = typeof(IValidator<>).MakeGenericType(queryType);
            var validators = serviceProvider.GetServices(validatorType).Cast<IValidator>().ToArray();

            if (validators.Length == 0)
                return;

            // Build ValidationContext<TQuery> at runtime
            var contextType = typeof(ValidationContext<>).MakeGenericType(queryType);
            var context = (IValidationContext)Activator.CreateInstance(contextType, query)!;

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
