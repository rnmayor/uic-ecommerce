using System.Net;
using Ecommerce.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Errors;

internal static class DomainExceptionMapper
{
    public static ProblemDetails Map(DomainException exception, string traceId)
    {
        // Domain validation or invariant violation: 400
        return ProblemDetailsFactory.Create(
          HttpStatusCode.BadRequest,
          "Domain Error",
          exception.Message,
          traceId
        );
    }
}
