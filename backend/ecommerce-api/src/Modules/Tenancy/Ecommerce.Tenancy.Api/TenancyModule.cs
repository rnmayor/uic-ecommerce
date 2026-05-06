using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Tenancy.Api
{
    public static class TenancyModule
    {
        public static IServiceCollection AddTenancyModule(this IServiceCollection services, IConfiguration configuration, string module)
        {
            if (!IsEnabled(configuration, module)) return services;

            return services;
        }

        public static WebApplication RegisterTenancyModule(this WebApplication app, string module)
        {
            if (!IsEnabled(app.Configuration, module)) return app;

            return app;
        }

        private static bool IsEnabled(IConfiguration configuration, string module)
        {
            return configuration.GetValue<bool>($"Modules:{module}:Enabled", true);
        }
    }
}
