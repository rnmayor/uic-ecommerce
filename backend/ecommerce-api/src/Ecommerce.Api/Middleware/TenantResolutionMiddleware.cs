using Ecommerce.Application.Common.Attributes;
using Ecommerce.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Middleware
{
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

            var endpoint = context.GetEndpoint();
            // Skip if endpoint explicitly opts out
            if (endpoint?.Metadata.GetMetadata<SkipTenantResolutionAttribute>() is not null)
            {
                await _next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue(TenantHeader, out var tenantIdHeader) || !Guid.TryParse(tenantIdHeader, out var tenantId))
            {
                var problem = new ProblemDetails
                {
                    Type = "api.invalid_tenant_context",
                    Title = "INVALID TENANT CONTEXT",
                    Detail = "Missing or invalid X-Tenant-Id header.",
                    Status = StatusCodes.Status400BadRequest,
                    Instance = context.Request.Path
                };

                problem.Extensions["traceId"] = context.TraceIdentifier;

                context.Response.StatusCode = problem.Status.Value;
                context.Response.ContentType = "application/problem+json";

                await context.Response.WriteAsJsonAsync(problem);

                return;
            }

            tenantContext.SetTenant(tenantId);

            await _next(context);
        }
    }
}