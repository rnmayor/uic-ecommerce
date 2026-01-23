using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Errors;

internal static class ProblemDetailsFactory
{
    public static ProblemDetails Create(
      HttpStatusCode status,
      string title,
      string detail,
      string traceId)
    {
        return new ProblemDetails
        {
            Status = (int)status,
            Title = title,
            Detail = detail,
            Extensions =
      {
        ["traceId"] = traceId
      }
        };
    }
}
