namespace SimpleMcpServer;

public class BearerTokenMiddleware
{
    private readonly RequestDelegate _next;
    private const string ExpectedKey = "your-secret"; // Securely load from configuration

    public BearerTokenMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var authHeader = context.Request.Headers.Authorization.ToString();
        if (authHeader != $"Bearer {ExpectedKey}")
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized: Invalid or missing bearer token.");
            return;
        }

        await _next(context);
    }
}