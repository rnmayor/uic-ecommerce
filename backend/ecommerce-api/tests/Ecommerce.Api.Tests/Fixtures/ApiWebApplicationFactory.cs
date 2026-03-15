using Ecommerce.Api.Configurations;
using Ecommerce.Application.Common.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Api.Tests.Fixtures
{
    public sealed class ApiWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseSetting($"{ClerkAuthOptions.SectionName}:Issuer", "https://dummy-issuer.com");
            builder.UseSetting($"{ClerkAuthOptions.SectionName}:Audience", "ecommerce-api");

            builder.UseSetting($"{DatabaseOptions.SectionName}:ConnectionString", "Server=localhost;Database=TestDb;");

            builder.ConfigureServices(services =>
            {
                services
                    .AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                        "Test", _ => { }
                    );
            });
        }
    }
}