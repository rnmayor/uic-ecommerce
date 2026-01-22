using Microsoft.AspNetCore.Authorization;

namespace Ecommerce.Application.Common.Authorization;

public sealed class TenantMemberRequirement : IAuthorizationRequirement
{
    public string[] AllowedRoles { get; }
    public TenantMemberRequirement(params string[] allowedRoles)
    {
        AllowedRoles = allowedRoles;
    }
}
