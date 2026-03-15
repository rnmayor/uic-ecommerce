using Ecommerce.Api.Configurations;
using Ecommerce.Api.Identity;
using Ecommerce.Application.Common.Interfaces;
using Ecommerce.Application.Common.Options;
using Ecommerce.Infrastructure.Tenancy;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;

namespace Ecommerce.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers and validates database configuration via options pattern.
        /// </summary>
        public static IServiceCollection AddDatabaseOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddOptions<DatabaseOptions>()
                .Bind(configuration.GetSection(DatabaseOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            return services;
        }

        /// <summary>
        /// Registers authentication:
        /// <list type="bullet">
        /// <item>External identity provider (Clerk) configuration via options pattern.</item>
        /// <item>JWT bearer authentication configuration and token validation behavior.</item>
        /// <item>ASP.Net Core authentication pipeline integration.</item>
        /// </list>
        /// </summary>
        public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<ClerkAuthOptions>()
                .Bind(configuration.GetSection(ClerkAuthOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();
            services.AddSingleton<IConfigureOptions<JwtBearerOptions>, ClerkJwtBearerOptionsSetup>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

            return services;
        }

        /// <summary>
        /// Registers identity components:
        /// <list type="bullet">
        /// <item><c>IClaimsTransformation:</c> Enriches authenticated principals with application-specific claims.</item>
        /// </list>
        /// </summary>
        public static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            services.AddScoped<IClaimsTransformation, ClaimsTransformation>();

            return services;
        }

        /// <summary>
        /// Registers tenancy-related information:
        /// <list type="bullet">
        /// <item><c>ITenantContext</c>: Holds the current tenant for the HTTP request, ensuring request isolation.</item>
        /// </list>
        /// </summary>
        public static IServiceCollection AddTenancy(this IServiceCollection services)
        {
            services.AddScoped<ITenantContext, TenantContext>();

            return services;
        }

        /// <summary>
        /// Registers API controllers and FluentValidation validators.
        /// </summary>
        public static IServiceCollection AddApiControllers(this IServiceCollection services)
        {
            services.AddControllers();

            // Register all validators in API assembly
            services.AddValidatorsFromAssemblyContaining<IApplicationMarker>();

            return services;
        }

        /// <summary>
        /// Registers Swagger/OpenAPI documentation generation and configuration.
        /// </summary>
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Ecommerce API",
                    Version = "v1"
                });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization Header"
                });
                options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                { new OpenApiSecuritySchemeReference("Bearer", document), new List<string>() }
                });
            });

            return services;
        }
    }
}