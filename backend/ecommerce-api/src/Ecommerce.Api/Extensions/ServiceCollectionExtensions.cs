using Ecommerce.Api.Configurations;
using Ecommerce.Api.Identity;
using Ecommerce.Application.Admin.Tenants.Membership;
using Ecommerce.Application.Admin.Tenants.Onboarding;
using Ecommerce.Application.Common.Interfaces;
using Ecommerce.Application.Common.Mapping;
using Ecommerce.Application.Common.Options;
using Ecommerce.Infrastructure.Authorization;
using Ecommerce.Infrastructure.Identity;
using Ecommerce.Infrastructure.Persistence.Membership;
using Ecommerce.Infrastructure.Persistence.Onboarding;
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

    /// <summary>
    /// Registers application-layer services that implement business use-cases and persistence abstractions required by those use-cases.
    /// <list type="bullet">
    /// <item><c>ITenantOnboardingRepository: </c> Provides persistence abstraction for atomically storing the entities created during tenant onboarding.</item>
    /// <item><c>ICreateTenantService: </c> Orchestrates the tenant onboarding use-case,  coordinating the creation of a Tenant, its initial Store, and the owning TenantUser.</item>
    /// <item><c>ITenantMembershipReadRepository: </c> Provides read-only persistence abstraction for querying tenant memberships for a given user, including role-based projections required by application-level queries.</item>
    /// <item><c>IMyTenantService: </c> Implements the "View my Tenants" use-case, returning the list of tenants the current user belongs to along with relevant membership details.</item>
    /// </list>
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ITenantOnboardingRepository, TenantOnboardingRepository>();
        services.AddScoped<ICreateTenantService, CreateTenantService>();
        services.AddScoped<ITenantMembershipReadRepository, TenantMembershipReadRepository>();
        services.AddScoped<IMyTenantService, MyTenantService>();

        return services;
    }

    /// <summary>
    /// Registers AutoMapper for the application.
    ///
    /// AutoMapper is used to map between domain models, read models,
    /// and API-facing DTOs. Mapping configuration is intentionally
    /// strict (field mapping disabled) to avoid implicit or accidental
    /// projections and to keep API contracts explicit.
    ///
    /// Mapping profiles live in the Application layer and are added
    /// incrementally per feature.
    /// </summary>
    public static IServiceCollection AddAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg =>
        {
            cfg.ShouldMapField = _ => false;
        }, typeof(ApplicationMappingProfile));

        return services;
    }
}
