using System.Security.Claims;
using Ecommerce.Application.Common.Tenancy;
using Microsoft.AspNetCore.Authorization;

namespace Ecommerce.Application.Common.Authorization;

public sealed class TenantMemberAuthorizationHandler : AuthorizationHandler<TenantMemberRequirement>
{
    private readonly ITenantContext _tenantContext;
    private readonly ITenantAuthorizationService _authService;
    public TenantMemberAuthorizationHandler(
      ITenantContext tenantContext,
      ITenantAuthorizationService authService
    )
    {
        _tenantContext = tenantContext;
        _authService = authService;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, TenantMemberRequirement requirement)
    {
        var clerkUserId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(clerkUserId))
        {
            return;
        }
        if (!_tenantContext.IsResolved)
        {
            return;
        }
        // Internal UserId already resolved via IClaimsTransformation
        var userIdClaim = context.User.FindFirst("user_id");
        if (userIdClaim is null)
        {
            return;
        }

        var userId = Guid.Parse(userIdClaim.Value);
        var isAuthorized = await _authService.IsTenantMemberAsync(_tenantContext.TenantId, userId, requirement.AllowedRoles);
        if (isAuthorized)
        {
            context.Succeed(requirement);
        }
    }
}
