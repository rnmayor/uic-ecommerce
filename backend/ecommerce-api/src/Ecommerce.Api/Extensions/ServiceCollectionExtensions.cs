using Ecommerce.Api.Configurations;
using Ecommerce.Api.Identity;
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
    /// Registers authentication infrastructure:
    /// <list type="bullet">
    /// <item>External identity provider (Clerk) configuration via options pattern.</item>
    /// <item>JWT bearer authentication configuration and token validation behavior.</item>
    /// <item>ASP.Net Core authentication pipeline integration.</item>
    /// </list>
    /// </summary>
    public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
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
    /// Registers tenancy-related infrastructure and cross-cutting support components:
    /// <list type="bullet">
    /// <item><c>ITenantContext</c>: Holds the current tenant for the HTTP request, ensuring request isolation.</item>
    /// <item><c>ITenantMemberAuthorizationService</c>: Performs tenant membership and role checks for authorization policies.</item>
    /// </list>
    /// </summary>
    public static IServiceCollection AddTenancyInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ITenantContext, TenantContext>();
        services.AddScoped<ITenantMemberAuthorizationService, TenantMemberAuthorizationService>();

        return services;
    }

    /// <summary>
    /// Registers identity infrastructure components:
    /// <list type="bullet">
    /// <item><c>IUserResolver:</c> Resolves the application's internal user identity from external authentication claims.</item>
    /// <item><c>IClaimsTransformation:</c> Enriches authenticated principals with application-specific claims.</item>
    /// </list>
    /// </summary>
    public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IUserResolver, UserResolver>();
        services.AddScoped<IClaimsTransformation, ClaimsTransformation>();

        return services;
    }
}
