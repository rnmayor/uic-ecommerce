using Ecommerce.Api.Middleware;

namespace Ecommerce.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandling(
      this IApplicationBuilder app) => app.UseMiddleware<GlobalExceptionMiddleware>();
}
