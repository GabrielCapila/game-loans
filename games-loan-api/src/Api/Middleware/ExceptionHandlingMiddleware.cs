using GamesLoan.Application.Exceptions;
using GamesLoan.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace GamesLoan.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (DomainException ex)
        {
            _logger.LogWarning(ex.Message, "Domain error");
            await HandleExceptionAsync(context,  ex);
        }
        catch (UnauthorizedException ex)
        {
            _logger.LogWarning(ex.Message,"Unauthorized");
            await HandleExceptionAsync(context,  ex);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex.Message,"Not found");
            await HandleExceptionAsync(context,  ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await HandleExceptionAsync(context,  ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        var message = "An unexpected error occurred.";

        switch (ex)
        {
            case NotFoundException nf:
                statusCode = HttpStatusCode.NotFound;
                message = nf.Message;
                break;

            case DomainException de:
                statusCode = HttpStatusCode.Conflict;
                message = de.Message;
                break;
            case UnauthorizedException ue:
                statusCode = HttpStatusCode.Unauthorized;
                message = ue.Message;
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var payload = JsonSerializer.Serialize(new
        {
            statusCode = context.Response.StatusCode,
            error = message
        });

        return context.Response.WriteAsync(payload);
    }
}