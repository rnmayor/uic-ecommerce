using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Ecommerce.Api.Errors
{
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
        {
            Log.Error(exception, "An unhandled exception occurred while processing the request.");

            var problem = new ProblemDetails
            {
                Type = "api.internal_server_error",
                Title = "INTERNAL SERVER ERROR",
                Detail = "An unhandled exception occurred while processing the request. Please try again later.",
                Status = StatusCodes.Status500InternalServerError,
                Instance = context.Request.Path
            };

            problem.Extensions["traceId"] = context.TraceIdentifier;

            context.Response.StatusCode = problem.Status.Value;
            context.Response.ContentType = "application/problem+json";

            await context.Response.WriteAsJsonAsync(problem, ct);

            return true;
        }
    }
}
