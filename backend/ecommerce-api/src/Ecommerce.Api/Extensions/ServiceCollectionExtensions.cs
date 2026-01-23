using Ecommerce.Api.Security;
using Ecommerce.Application.Common.Interfaces;
using Ecommerce.Infrastructure.Authorization;
using Ecommerce.Infrastructure.Identity;
using Ecommerce.Infrastructure.Tenancy;
using Microsoft.AspNetCore.Authentication;

namespace Ecommerce.Api.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers tenant-related services (request-scoped):
    /// <list type="bullet">
    /// <item><c>ITenantContext</c>: Holds the current tenant for the HTTP request, ensuring isolation between requests.</item>
    /// <item><c>ITenantMemberAuthorizationService</c>: Checks if the current user is a member of the tenant with the allowed roles (Authorization Policy).</item>
    /// </list>
    /// </summary>
    public static IServiceCollection AddTenantServices(this IServiceCollection services)
    {
        services.AddScoped<ITenantContext, TenantContext>();
        services.AddScoped<ITenantMemberAuthorizationService, TenantMemberAuthorizationService>();
        return services;
    }

    /// <summary>
    /// Registers user-related services (request-scoped):
    /// <list type="bullet">
    /// <item><c>IUserResolver:</c> Resolves the application's internal user ID from Clerk's user ID and maps it to local User entity.</item>
    /// <item><c>IClaimsTransformation:</c> Add claims (like user_id) to the authenticated user for downstream services and policies.</item>
    /// </list>
    /// </summary>
    public static IServiceCollection AddUserServices(this IServiceCollection services)
    {
        services.AddScoped<IUserResolver, UserResolver>();
        services.AddScoped<IClaimsTransformation, ClaimsTransformation>();
        return services;
    }
}
