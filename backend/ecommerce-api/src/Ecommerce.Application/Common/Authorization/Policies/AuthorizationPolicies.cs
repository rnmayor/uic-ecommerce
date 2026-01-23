using Ecommerce.Application.Common.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace Ecommerce.Application.Common.Authorization.Policies;

public static class AuthorizationPolicies
{
    public const string TenantAdmin = "TenantAdmin";
    public static void AddPolicies(AuthorizationOptions options)
    {
        options.AddPolicy(TenantAdmin, policy =>
          policy.Requirements.Add(new TenantMemberRequirement("Owner", "Admin")));
    }

}
