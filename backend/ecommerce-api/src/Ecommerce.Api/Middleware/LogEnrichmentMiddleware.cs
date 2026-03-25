using Ecommerce.Application.Common.Interfaces;
using Serilog.Context;
using Serilog.Core;
using Serilog.Core.Enrichers;
using System.Security.Claims;

namespace Ecommerce.Api.Middleware
{
    public sealed class LogEnrichmentMiddleware
    {
        private readonly RequestDelegate _next;
        public LogEnrichmentMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ITenantContext tenantContext)
        {
            var enrichers = new List<ILogEventEnricher>
            {
                new PropertyEnricher("TraceId", context.TraceIdentifier)
            };

            var userId = context.User.FindFirst("user_id")?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                enrichers.Add(new PropertyEnricher("UserId", userId));
            }

            var clerkUserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(clerkUserId))
            {
                enrichers.Add(new PropertyEnricher("ClerkUserId", clerkUserId));
            }

            if (tenantContext.IsResolved)
            {
                enrichers.Add(new PropertyEnricher("TenantId", tenantContext.TenantId));
            }

            using (LogContext.Push(enrichers.ToArray()))
            {
                await _next(context);
            }
        }
    }
}