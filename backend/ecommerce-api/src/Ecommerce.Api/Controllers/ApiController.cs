using Ecommerce.Api.Extensions;
using Ecommerce.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers
{
    [ApiController]
    public abstract class ApiController : ControllerBase
    {
        protected ActionResult HandleResult<T>(Result<T> result, Func<T, ActionResult> onSuccess)
        {
            return result.ToActionResult(HttpContext, onSuccess);
        }
    }
}
