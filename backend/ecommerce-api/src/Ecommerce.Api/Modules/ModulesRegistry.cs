using Ecommerce.Api.Extensions;
using Ecommerce.Api.Modules.Tenancy;

namespace Ecommerce.Api.Modules
{
    internal static class ModulesRegistry
    {
        internal static void AddModules(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTenanyModule(configuration, Module.Tenancy);
        }

        internal static void RegisterModules(this WebApplication app)
        {
            app.RegisterTenancyModule(Module.Tenancy);
        }
    }
}
