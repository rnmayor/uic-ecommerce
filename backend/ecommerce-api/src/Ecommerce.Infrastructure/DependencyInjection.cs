using Ecommerce.Application.Common.Persistence;
using Ecommerce.Application.Admin.Stores.Brands.GetAll;
using Ecommerce.Application.Admin.Tenants.Membership.GetMyTenants;
using Ecommerce.Application.Common.Interfaces;
using Ecommerce.Application.Common.Options;
using Ecommerce.Infrastructure.Authorization;
using Ecommerce.Infrastructure.Identity;
using Ecommerce.Infrastructure.Persistence;
using Ecommerce.Infrastructure.Persistence.Repositories.Stores.Brands.GetAll;
using Ecommerce.Infrastructure.Persistence.Repositories.Tenants;
using Ecommerce.Infrastructure.Persistence.Repositories.Tenants.Membership.GetMyTenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Ecommerce.Application.Admin.Tenants.Onboarding;
using Ecommerce.Infrastructure.Persistence.Repositories.Tenants.Onboarding;
using Ecommerce.Infrastructure.Persistence.Repositories.Stores.Brands;

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
    /// <item><c>IGetMyTenantsRepository: </c> Provides read-only persistence abstraction for querying tenant memberships for a given user, including role-based projections required by application-level queries.</item>
    /// <item><c>IGetAllStoreBrandsRepository: </c> Provides read-only persistence abstraction for querying store brands.</item>
    /// <item><c>ITenantRepository: </c> Provides persistence abstraction for storing tenant.</item>
    /// <item><c>IStoreBrandRepository: </c> Provides persistence abstraction for storing store brand.</item>
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

        // Use-case specific repositories
        services.AddScoped<IOnboardingRepository, OnboardingRepository>();
        services.AddScoped<IGetMyTenantsRepository, GetMyTenantsRepository>();
        services.AddScoped<IGetAllStoreBrandsRepository, GetAllStoreBrandsRepository>();

        // Aggregrate repositories
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IStoreBrandRepository, StoreBrandRepository>();

        return services;
    }
}
