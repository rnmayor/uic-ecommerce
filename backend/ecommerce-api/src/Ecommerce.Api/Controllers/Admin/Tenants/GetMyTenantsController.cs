using Ecommerce.Api.Extensions;
using Ecommerce.Application.Admin.Tenants.Membership.GetMyTenants;
using Ecommerce.Application.Common.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers.Admin.Tenants;

[ApiController]
[Route("api/admin/me")]
[Authorize]
[SkipTenantResolution]
public sealed class GetMyTenantsController : ControllerBase
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
        var response = await _service.HandleAsync(userId, ct);

        return Ok(response);
    }
}
