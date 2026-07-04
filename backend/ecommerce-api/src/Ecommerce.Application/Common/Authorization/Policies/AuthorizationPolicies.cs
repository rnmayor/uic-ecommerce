using Ecommerce.Application.Common.Authorization.Requirements;
using Ecommerce.Domain.Tenants;
using Microsoft.AspNetCore.Authorization;

namespace Ecommerce.Application.Common.Authorization.Policies;

/// <summary>
/// Central registry for authorization policies used by the application.
/// Policies are enforced via custom authorization handlers and requirements.
/// </summary>
public static class AuthorizationPolicies
{
    /// <summary>
    /// Allow users who are members of the current tenant
    /// with either "Owner" or "Admin" role.
    /// </summary>
    public const string TenantAdmin = "TenantAdmin";

    /// <summary>
    /// Registers all application authorization policies.
    /// Called during application startup.
    /// </summary>
    /// <param name="options"></param>
    public static void AddPolicies(AuthorizationOptions options)
    {
        options.AddPolicy(TenantAdmin, policy =>
            policy.Requirements.Add(new TenantMemberRequirement(TenantRoles.Owner, TenantRoles.Admin)));
    }

}
