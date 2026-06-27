using System.Security.Claims;
using LibraryManagement.Core.Entities;
using LibraryManagement.Infrastructure.Data;

namespace LibraryManagement.API.Middleware;

public class ActivityLoggingMiddleware:IMiddleware
{
    private static readonly HashSet<string> ReadMethods = new(StringComparer.OrdinalIgnoreCase) { "GET", "HEAD", "OPTIONS" };
    private readonly LibraryDbContext dbContext;

    public ActivityLoggingMiddleware(LibraryDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (ReadMethods.Contains(context.Request.Method) || !context.User.Identity?.IsAuthenticated == true)
        {
            await next(context);
            return;
        }

        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await next(context);

            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim != null && int.TryParse(userIdClaim, out var userId) && userId > 0)
            {
                var log = new ActivityLog
                {
                    UserId = userId,
                    Action = $"{context.Request.Method} {context.Request.Path}",
                    EntityType = ExtractEntityType(context.Request.Path),
                    EntityId = ExtractEntityId(context.Request.Path),
                    Details = $"Status: {context.Response.StatusCode}",
                    Timestamp = DateTime.UtcNow,
                    IpAddress = context.Connection.RemoteIpAddress?.ToString()
                };

                dbContext.ActivityLogs.Add(log);
                await dbContext.SaveChangesAsync();
            }

            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }

    private static string ExtractEntityType(string path)
    {
        var segments = path.Trim('/').Split('/');
        return segments.Length > 1 ? segments[1] : "unknown";
    }

    private static int? ExtractEntityId(string path)
    {
        var segments = path.Trim('/').Split('/');
        if (segments.Length > 2 && int.TryParse(segments[2], out var id))
            return id;
        return null;
    }
}
