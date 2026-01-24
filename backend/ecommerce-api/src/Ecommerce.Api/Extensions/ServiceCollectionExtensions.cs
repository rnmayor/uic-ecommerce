using Ecommerce.Api.Configurations;
using Ecommerce.Api.Security;
using Ecommerce.Application.Common.Interfaces;
using Ecommerce.Application.Common.Options;
using Ecommerce.Infrastructure.Authorization;
using Ecommerce.Infrastructure.Identity;
using Ecommerce.Infrastructure.Tenancy;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace Ecommerce.Api.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers and validates database configuration via options pattern.
    /// </summary>
    public static IServiceCollection AddDatabaseOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<DatabaseOptions>()
            .Configure(options =>
            {
                options.ConnectionString =
                     configuration.GetConnectionString("Database")
                     ?? throw new InvalidOperationException(
                         "Database connection string is missing."
                     );
            })
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }

    /// <summary>
    /// Registers authentication services:
    /// <list type="bullet">
    /// <item>Clerk configuration via options pattern</item>
    /// <item>JwtBearerOptions configuration using Clerk settings</item>
    /// <item>JWT Bearer authentication handler</item>
    /// </list>
    /// </summary>
    public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<ClerkAuthOptions>()
            .Bind(configuration.GetSection(ClerkAuthOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddSingleton<IConfigureOptions<JwtBearerOptions>, ClerkJwtBearerOptionsSetup>();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

        return services;
    }

    /// <summary>
    /// Registers tenant-related services:
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
    /// Registers user-related services:
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
