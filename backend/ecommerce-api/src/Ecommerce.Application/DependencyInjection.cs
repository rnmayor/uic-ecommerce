using Ecommerce.Application.Admin.Tenants.Membership;
using Ecommerce.Application.Admin.Tenants.Onboarding;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Application;

public static class DependencyInjection
{
    /// <summary>
    /// Registers application-layer services that implement business use-cases
    /// <list type="bullet">
    /// <item><c>ICreateTenantService: </c> Orchestrates the tenant onboarding use-case, coordinating the creation of a Tenant, its initial Store, and the owning TenantUser.</item>
    /// <item><c>IMyTenantService: </c> Returns the list of tenants the current user belongs to along with relevant membership details.</item>
    /// </list>
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICreateTenantService, CreateTenantService>();
        services.AddScoped<IMyTenantService, MyTenantService>();

        return services;
    }
}
