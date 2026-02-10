using Ecommerce.Api.Extensions;
using Ecommerce.Application.Admin.Tenants.Onboarding;
using Ecommerce.Application.Common.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/onboarding")]
[Authorize] // authenticated user only
[SkipTenantResolution]
public sealed class OnboardingController : ControllerBase
{
    private readonly ICreateTenantService _service;
    public OnboardingController(ICreateTenantService service)
    {
        _service = service;
    }

    [HttpPost("tenant")]
    public async Task<ActionResult<CreateTenantResponse>> CreateTenant(
      [FromBody] CreateTenantRequest request,
      CancellationToken ct
    )
    {
        var userId = User.GetUserId();
        var result = await _service.CreateAsync(userId, request, ct);

        return Created(string.Empty, result);
    }

    [HttpGet("ping")]
    public ActionResult Ping()
    {
        return Ok(new
        {
            status = "ok",
            service = "tenant-onboarding",
            timestamp = DateTimeOffset.UtcNow
        });
    }
}
