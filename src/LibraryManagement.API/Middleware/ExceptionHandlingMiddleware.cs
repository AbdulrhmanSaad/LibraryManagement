using System.Net;
using System.Text.Json;
using FluentValidation;

namespace LibraryManagement.API.Middleware;

public class ExceptionHandlingMiddleware : IMiddleware
{

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";

            var errors = ex.Errors.Select(e => new
            {
                field = e.PropertyName,
                message = e.ErrorMessage,
                attemptedValue = e.AttemptedValue?.ToString()
            });

            var result = JsonSerializer.Serialize(new { message = "Validation failed", errors });
            await context.Response.WriteAsync(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Response.ContentType = "application/json";
            var result = JsonSerializer.Serialize(new { message = ex.Message });
            await context.Response.WriteAsync(result);
        }
        catch (KeyNotFoundException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            context.Response.ContentType = "application/json";
            var result = JsonSerializer.Serialize(new { message = ex.Message });
            await context.Response.WriteAsync(result);
        }
        catch (InvalidOperationException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Conflict;
            context.Response.ContentType = "application/json";
            var result = JsonSerializer.Serialize(new { message = ex.Message });
            await context.Response.WriteAsync(result);
        }
    }
}
