using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Security;
using System.Text;
using System.Text.Json;
using TaskApp.Application;
using TaskApp.Domain.Exceptions;
using TaskApp.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("CorsPolicy", policy =>
    {
        policy
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .WithExposedHeaders("WWW-Authenticate", "Pagination")
            .WithOrigins("http://localhost:5173");
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token like this: Bearer {your token}"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });

});
builder.Services.AddHttpContextAccessor();

var jwt = builder.Configuration.GetSection("Jwt");
var key = jwt["Key"] ?? throw new InvalidOperationException("Jwt:Key missing");

builder.Services
    .AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.IncludeErrorDetails = true;
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt["Issuer"],

            ValidateAudience = true,
            ValidAudience = jwt["Audience"],

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };

    });

builder.Services.AddAuthorization();

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var logger = context.RequestServices
            .GetRequiredService<ILogger<Program>>();

        var feature = context.Features.Get<IExceptionHandlerFeature>();
        var ex = feature?.Error;

        if (ex is null)
            return;

        logger.LogError(ex, "Unhandled exception occurred");

        var traceId = context.TraceIdentifier;

        ProblemDetails problem;
        int statusCode;

        switch (ex)
        {
            case ValidationException vex:
                {
                    var errors = vex.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage).Distinct().ToArray()
                        );

                    var summary = string.Join(" | ", vex.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));

                    var validationProblem = new ValidationProblemDetails(errors)
                    {
                        Title = "Validation failed",
                        Status = StatusCodes.Status400BadRequest,
                        Detail = summary,
                        Instance = context.Request.Path,
                        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
                    };

                    validationProblem.Extensions["traceId"] = traceId;

                    statusCode = StatusCodes.Status400BadRequest;
                    problem = validationProblem;
                    break;
                }

            case UnauthorizedAccessException uex:
                {
                    statusCode = StatusCodes.Status401Unauthorized;

                    problem = new ProblemDetails
                    {
                        Title = "Unauthorized",
                        Status = statusCode,
                        Detail = uex.Message, // Optional: hide message in production
                        Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
                        Instance = context.Request.Path
                    };

                    problem.Extensions["traceId"] = traceId;
                    break;
                }

            case SecurityException:
                {
                    statusCode = StatusCodes.Status403Forbidden;

                    problem = new ProblemDetails
                    {
                        Title = "Forbidden",
                        Status = statusCode,
                        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                        Instance = context.Request.Path
                    };

                    problem.Extensions["traceId"] = traceId;
                    break;
                }

            case NotFoundException nf:
                {
                    statusCode = StatusCodes.Status404NotFound;

                    problem = new ProblemDetails
                    {
                        Title = "Resource not found",
                        Status = statusCode,
                        Detail = nf.Message,
                        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                        Instance = context.Request.Path
                    };

                    problem.Extensions["traceId"] = traceId;
                    break;
                }

            case DbUpdateConcurrencyException:
                {
                    statusCode = StatusCodes.Status409Conflict;

                    problem = new ProblemDetails
                    {
                        Title = "Concurrency conflict",
                        Status = statusCode,
                        Detail = "The resource was modified by another request.",
                        Instance = context.Request.Path
                    };

                    problem.Extensions["traceId"] = traceId;
                    break;
                }

            case DbUpdateException:
                {
                    statusCode = StatusCodes.Status409Conflict;

                    problem = new ProblemDetails
                    {
                        Title = "Database update failed",
                        Status = statusCode,
                        Detail = "A database constraint was violated.",
                        Instance = context.Request.Path
                    };

                    problem.Extensions["traceId"] = traceId;
                    break;
                }

            default:
                {
                    statusCode = StatusCodes.Status500InternalServerError;

                    problem = new ProblemDetails
                    {
                        Title = "An unexpected error occurred",
                        Status = statusCode,
                        Instance = context.Request.Path
                    };

                    problem.Extensions["traceId"] = traceId;

                    break;
                }
        }

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
    });
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
