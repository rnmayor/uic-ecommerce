using Ecommerce.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Extensions
{
    public static class ResultExtensions
    {
        public static ActionResult ToActionResult<T>(
            this Result<T> result,
            HttpContext context,
            Func<T, ActionResult> onSuccess)
        {
            if (result.IsSuccess)
                return onSuccess(result.Value);

            var error = result.Error;

            var problem = new ProblemDetails
            {
                Type = error.Code,
                Title = error.Code.Replace(".", " ").Replace("_", " ").ToUpperInvariant(),
                Detail = error.Description,
                Status = (int)error.StatusCode,
                Instance = context.Request.Path
            };

            problem.Extensions["traceId"] = context.TraceIdentifier;

            return new ObjectResult(problem)
            {
                StatusCode = (int)error.StatusCode
            };
        }
    }
}
