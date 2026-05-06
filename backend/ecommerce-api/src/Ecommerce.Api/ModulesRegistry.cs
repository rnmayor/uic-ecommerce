using Ecommerce.Tenancy.Api;

namespace Ecommerce.Api
{
    internal static class ModulesRegistry
    {
        internal static void AddModules(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTenancyModule(configuration, Module.Tenancy);
        }

        internal static void RegisterModules(this WebApplication app)
        {
            app.RegisterTenancyModule(Module.Tenancy);
        }
    }
}
