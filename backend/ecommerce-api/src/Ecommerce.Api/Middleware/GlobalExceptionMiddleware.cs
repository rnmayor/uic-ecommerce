using System.Net;
using System.Text.Json;
using Ecommerce.Api.Errors;
using Ecommerce.Domain.Common;
using Serilog;

namespace Ecommerce.Api.Middleware;

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = context.TraceIdentifier;
        Log.Error("Unhandled exception occured. TraceId: {traceId}", traceId);

        var problem = exception switch
        {
            DomainException domainEx => DomainExceptionMapper.Map(domainEx, traceId),

            UnauthorizedAccessException => ProblemDetailsFactory.Create(
                HttpStatusCode.Unauthorized,
                "Unauthorized",
                "You are not authorized to perform this action",
                traceId
            ),

            _ => ProblemDetailsFactory.Create(
              HttpStatusCode.InternalServerError,
              "Internal Server Error",
              "An unexpected error occured.",
              traceId
            )
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = problem.Status!.Value;

        var json = JsonSerializer.Serialize(problem);
        await context.Response.WriteAsync(json);
    }
}
