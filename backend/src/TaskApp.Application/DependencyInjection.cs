using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TaskApp.Application.Abstractions.Messaging;
using TaskApp.Application.Abstractions.Utils;
using TaskApp.Application.Common;
using TaskApp.Application.TodoTasks;
using TaskApp.Application.TodoTasks.Create;
using TaskApp.Application.TodoTasks.Delete;
using TaskApp.Application.TodoTasks.Details;
using TaskApp.Application.TodoTasks.List;
using TaskApp.Application.TodoTasks.MoveToNextStatus;
using TaskApp.Application.TodoTasks.Update;
using TaskApp.Application.Users;
using TaskApp.Application.Users.Login;
using TaskApp.Application.Users.Register;

namespace TaskApp.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<CreateTodoTaskCommandValidator>();
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // Register command and query handlers
            // Tasks
            services.AddScoped<ICommandHandler<CreateTodoTaskCommand, TodoTaskDto>, CreateTodoTaskCommandHandler>();
            services.AddScoped<ICommandHandler<DeleteTodoTaskCommand, bool>, DeleteTodoTaskCommandHandler>();
            services.AddScoped<IQueryHandler<GetTodoTaskDetailsQuery, TodoTaskDto>, GetTodoTaskDetailsQueryHandler>();
            services.AddScoped<IQueryHandler<GetTodoTasksQuery, PagedResult<TodoTaskResponse>>, GetTodoTasksQueryHandler>();
            services.AddScoped<ICommandHandler<MoveToNextStatusCommand, TodoTaskDto>, MoveToNextStatusCommandHandler>();
            services.AddScoped<ICommandHandler<UpdateTodoTaskCommand, TodoTaskDto>, UpdateTodoTaskCommandHandler>();

            // Users
            services.AddScoped<ICommandHandler<UserLoginCommand, AuthResultDto>, UserLoginCommandHandler>();
            services.AddScoped<ICommandHandler<RegisterUserCommand, AuthResultDto>, RegisterUserCommandHandler>();

            return services;
        }
    }
}
