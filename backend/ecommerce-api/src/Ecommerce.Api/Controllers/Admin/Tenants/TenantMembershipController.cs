using Ecommerce.Api.Extensions;
using Ecommerce.Application.Admin.Tenants.Membership;
using Ecommerce.Application.Common.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers.Admin.Tenants;

[ApiController]
[Route("api/admin/me")]
[Authorize]
[SkipTenantResolution]
public sealed class TenantMembershipController : ControllerBase
{
    private readonly IMyTenantService _service;
    public TenantMembershipController(IMyTenantService service)
    {
        _service = service;
    }

    [HttpGet("tenants")]
    public async Task<ActionResult<MyTenantResponse>> GetMyTenants(
      CancellationToken ct
    )
    {
        var userId = User.GetUserId();
        var response = await _service.GetMyTenantsAsync(userId, ct);

        return Ok(response);
    }

    [HttpGet("ping")]
    public ActionResult Ping()
    {
        return Ok(new
        {
            status = "ok",
            service = "tenant-membership",
            timestamp = DateTimeOffset.UtcNow
        });
    }

}
