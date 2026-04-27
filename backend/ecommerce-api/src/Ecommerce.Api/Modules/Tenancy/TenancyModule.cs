namespace Ecommerce.Api.Modules.Tenancy
{
    public static class TenancyModule
    {
        public static IServiceCollection AddTenanyModule(this IServiceCollection services, IConfiguration configuration, string module)
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
