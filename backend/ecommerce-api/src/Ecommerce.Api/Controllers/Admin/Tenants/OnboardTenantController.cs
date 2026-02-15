using Ecommerce.Api.Extensions;
using Ecommerce.Application.Admin.Tenants.Onboarding;
using Ecommerce.Application.Common.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers.Admin.Tenants;

[ApiController]
[Route("api/admin/onboarding")]
[Authorize] // authenticated user only
[SkipTenantResolution]
public sealed class OnboardTenantController : ControllerBase
{
    private readonly IOnboardingService _service;
    public OnboardTenantController(IOnboardingService service)
    {
        _service = service;
    }

    [HttpPost("tenant")]
    public async Task<ActionResult<OnboardingResponse>> HandleAsync(
      [FromBody] OnboardingRequest request,
      CancellationToken ct
    )
    {
        var userId = User.GetUserId();
        var result = await _service.ExecuteAsync(userId, request, ct);

        return Created(string.Empty, result);
    }
}
