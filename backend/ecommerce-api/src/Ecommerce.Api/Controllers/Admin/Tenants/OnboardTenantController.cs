using Ecommerce.Api.Extensions;
using Ecommerce.Application.Admin.Tenants.Features.Onboarding;
using Ecommerce.Application.Common.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers.Admin.Tenants
{
    [Route("api/admin/onboarding")]
    [Authorize] // authenticated user only
    [SkipTenantResolution]
    public sealed class OnboardTenantController : ApiController
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

            return HandleResult(result, tenant => Created(string.Empty, tenant));
        }
    }
}