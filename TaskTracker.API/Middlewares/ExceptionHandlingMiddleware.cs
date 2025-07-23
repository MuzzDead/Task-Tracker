using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.Exceptions;

namespace TaskTracker.API.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            if (context.Response.HasStarted)
            {
                _logger.LogWarning("Cannot write error response. Response already started.");
                throw;
            }

            context.Response.Clear();
            context.Response.ContentType = "application/problem+json";

            var (statusCode, title, logLevel) = ex switch
            {
                UnauthorizedException => (StatusCodes.Status401Unauthorized, "Unauthorized", LogLevel.Information),
                ConflictException => (StatusCodes.Status409Conflict, "Conflict", LogLevel.Warning),
                NotFoundException => (StatusCodes.Status404NotFound, "Not Found", LogLevel.Warning),
                ValidationException => (StatusCodes.Status400BadRequest, "Validation Error", LogLevel.Warning),
                _ => (StatusCodes.Status500InternalServerError, "Internal Server Error", LogLevel.Error)
            };

            _logger.Log(logLevel, ex, "Handled exception: {Message}", ex.Message);

            context.Response.StatusCode = statusCode;

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = ex.Message,
                Type = $"https://httpstatuses.com/{statusCode}"
            };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, options));
        }
    }
}
