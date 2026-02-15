using Ecommerce.Application.Admin.Stores.Brands.GetAll;
using Ecommerce.Application.Admin.Tenants.Membership.GetMyTenants;
using Ecommerce.Application.Admin.Tenants.Onboarding;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Application;

public static class DependencyInjection
{
    /// <summary>
    /// Registers application-layer services that implement business use-cases
    /// <list type="bullet">
    /// <item><c>IOnboardingService: </c> Orchestrates the tenant onboarding use-case, coordinating the creation of a Tenant and the owning TenantUser.</item>
    /// <item><c>IGetMyTenantsService: </c> Returns the list of tenants the current user belongs to along with relevant membership details.</item>
    /// <item><c>IGetAllStoreBrandsService: </c> Returns the list of store-brands.</item>
    /// </list>
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IOnboardingService, OnboardingService>();
        services.AddScoped<IGetMyTenantsService, GetMyTenantsService>();
        services.AddScoped<IGetAllStoreBrandsService, GetAllStoreBrandsService>();

        return services;
    }
}
