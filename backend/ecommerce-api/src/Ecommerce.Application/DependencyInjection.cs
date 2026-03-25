using Ecommerce.Application.Admin.Stores.Brands.Commands.CreateStoreBrand;
using Ecommerce.Application.Admin.Stores.Brands.Queries.GetAllStoreBrands;
using Ecommerce.Application.Admin.Tenants.Features.Onboarding;
using Ecommerce.Application.Admin.Tenants.Queries.GetMyTenants;
using Ecommerce.Application.Admin.Tenants.Queries.GetTenant;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Application
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Registers application-layer services that implement business use-cases
        /// <list type="bullet">
        /// <item><c>IOnboardingService: </c> Orchestrates the tenant onboarding use-case, coordinating the creation of a Tenant and the owning TenantUser.</item>
        /// <item><c>IGetMyTenantsService: </c> Returns the list of tenants the current user belongs to along with relevant membership details.</item>
        /// <item><c>IGetTenantService: </c> Returns tenant details for a given slug.</item>
        /// <item><c>IGetAllStoreBrandsService: </c> Returns the list of store-brands.</item>
        /// <item><c>ICreateStoreBrandService: </c> Creates store-brand.</item>
        /// </list>
        /// </summary>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IOnboardingService, OnboardingService>();
            services.AddScoped<IGetMyTenantsService, GetMyTenantsService>();
            services.AddScoped<IGetTenantService, GetTenantService>();
            services.AddScoped<IGetAllStoreBrandsService, GetAllStoreBrandsService>();
            services.AddScoped<ICreateStoreBrandService, CreateStoreBrandService>();

            return services;
        }
    }
}