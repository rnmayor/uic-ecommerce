using Ecommerce.Api.Middleware;

namespace Ecommerce.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// CorrelationIdMiddleware: Sets trace identifier that travels with a request and shared across logs, errors, and downstream calls.
    /// </summary>
    public static IApplicationBuilder UseCorrelationId(
      this IApplicationBuilder app) => app.UseMiddleware<CorrelationIdMiddleware>();

    /// <summary>
    /// TenantResolutionMiddleware: Resolves the current tenant based on the authenticated user.
    /// </summary>
    public static IApplicationBuilder UseTenantResolution(
      this IApplicationBuilder app) => app.UseMiddleware<TenantResolutionMiddleware>();

    /// <summary>
    /// LogEnrichmentMiddleware: Adds structured context (UserId, ClerkUserId, TenantId, TraceId) to logs.
    /// Enables centralized and consistent logging for diagnostics.
    /// </summary>
    public static IApplicationBuilder UseLogEnrichment(
      this IApplicationBuilder app) => app.UseMiddleware<LogEnrichmentMiddleware>();

    /// <summary>
    /// GlobalExceptionMiddleware: Catches exceptions from all downstream middleware.
    /// Returns consistent JSON problem responses with correlation IDs for traceability.
    /// </summary>
    public static IApplicationBuilder UseGlobalExceptionHandling(
      this IApplicationBuilder app) => app.UseMiddleware<GlobalExceptionMiddleware>();
}
