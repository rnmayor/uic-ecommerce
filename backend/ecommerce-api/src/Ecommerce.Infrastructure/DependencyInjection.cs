using Ecommerce.Application.Admin.Stores.Brands;
using Ecommerce.Application.Admin.Tenants.Membership;
using Ecommerce.Application.Admin.Tenants.Onboarding;
using Ecommerce.Application.Common.Interfaces;
using Ecommerce.Application.Common.Options;
using Ecommerce.Infrastructure.Authorization;
using Ecommerce.Infrastructure.Identity;
using Ecommerce.Infrastructure.Persistence;
using Ecommerce.Infrastructure.Persistence.Membership;
using Ecommerce.Infrastructure.Persistence.Onboarding;
using Ecommerce.Infrastructure.Persistence.Repositories.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Ecommerce.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Registers infrastructure-layer implementation for persistence, identity,
    /// and tenant-based authorization required by the application layer.
    /// <item><c>EcommerceDbContext</c>: Configures connection string, strongly-typed options, and database provider.</item>
    /// <item><c>ITenantMemberAuthorizationService</c>: Performs tenant membership and role checks for authorization policies.</item>
    /// <item><c>IUserResolver:</c> Resolves the application's internal user identity from external authentication claims.</item>
    /// <item><c>ITenantOnboardingRepository: </c> Provides persistence abstraction for atomically storing the entities created during tenant onboarding.</item>
    /// <item><c>ITenantMembershipReadRepository: </c> Provides read-only persistence abstraction for querying tenant memberships for a given user, including role-based projections required by application-level queries.</item>
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // DbContext
        services.AddDbContext<EcommerceDbContext>((sp, options) =>
        {
            var dbOptions = sp.GetRequiredService<IOptions<DatabaseOptions>>().Value;
            options.UseNpgsql(dbOptions.ConnectionString);
        });

        // Tenancy
        services.AddScoped<ITenantMemberAuthorizationService, TenantMemberAuthorizationService>();

        // Identity
        services.AddScoped<IUserResolver, UserResolver>();

        // Repositories
        services.AddScoped<ITenantOnboardingRepository, TenantOnboardingRepository>();
        services.AddScoped<ITenantMembershipReadRepository, TenantMembershipReadRepository>();
        services.AddScoped<IStoreBrandReadRepository, StoreBrandReadRepository>();

        return services;
    }
}
