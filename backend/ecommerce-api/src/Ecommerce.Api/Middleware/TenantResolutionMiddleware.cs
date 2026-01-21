using Ecommerce.Application.Common.Tenancy;

namespace Ecommerce.Api.Middleware;

public sealed class TenantResolutionMiddleware
{
    private const string TenantHeader = "X-Tenant-Id";
    private readonly RequestDelegate _next;

    public TenantResolutionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantContext tenantContext)
    {
        // Skip unauthenticated requests (Swagger, health checks, etc)
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(TenantHeader, out var tenantIdHeader) || !Guid.TryParse(tenantIdHeader, out var tenantId))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("Missing or invalid X-Tenant-Id header.");
            return;
        }

        tenantContext.SetTenant(tenantId);

        await _next(context);
    }
}
