using System.Security.Claims;
using Ecommerce.Application.Common.Tenancy;
using Serilog.Context;

namespace Ecommerce.Api.Logging;

public sealed class LogEnrichmentMiddleware
{
    private readonly RequestDelegate _next;
    public LogEnrichmentMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantContext tenantContext)
    {
        var userId = context.User.FindFirst("user_id")?.Value;
        var clerkUserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var tenantId = tenantContext.TenantId.ToString();

        using (LogContext.PushProperty("UserId", userId)) ;
        using (LogContext.PushProperty("ClerkUserId", clerkUserId)) ;
        using (LogContext.PushProperty("TenantId", tenantId))
        using (LogContext.PushProperty("TraceId", context.TraceIdentifier))
        {
            await _next(context);
        }
    }
}
