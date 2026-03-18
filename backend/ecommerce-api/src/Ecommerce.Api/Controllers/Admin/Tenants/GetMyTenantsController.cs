using Ecommerce.Api.Extensions;
using Ecommerce.Application.Admin.Tenants.Queries.GetMyTenants;
using Ecommerce.Application.Common.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers.Admin.Tenants
{
    [Route("api/admin/me")]
    [Authorize]
    [SkipTenantResolution]
    public sealed class GetMyTenantsController : ApiController
    {
        private readonly IGetMyTenantsService _service;
        public GetMyTenantsController(IGetMyTenantsService service)
        {
            _service = service;
        }

        [HttpGet("tenants")]
        public async Task<ActionResult<MyTenantsResponse>> HandleAsync(
          CancellationToken ct
        )
        {
            var userId = User.GetUserId();
            var result = await _service.HandleAsync(userId, ct);

            return HandleResult(result, myTenants => Ok(myTenants));
        }
    }
}