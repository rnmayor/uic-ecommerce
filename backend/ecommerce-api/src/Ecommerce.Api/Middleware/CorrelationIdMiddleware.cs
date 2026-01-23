namespace Ecommerce.Api.Middleware;

public sealed class CorrelationIdMiddleware
{
    public const string CorrelationIdHeader = "X-Correlation-Id";
    private readonly RequestDelegate _next;
    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Read incoming CorrelationId (if any)
        var correlationId = context.Request.Headers.TryGetValue(
          CorrelationIdHeader,
          out var existingCorrelationId)
          ? existingCorrelationId.ToString()
          : Guid.NewGuid().ToString();

        // Set TraceIdentifier (ASP.Net Core standard)
        context.TraceIdentifier = correlationId;
        // Ensure response contains the CorrelationId
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[CorrelationIdHeader] = correlationId;
            return Task.CompletedTask;
        });

        await _next(context);
    }
}
