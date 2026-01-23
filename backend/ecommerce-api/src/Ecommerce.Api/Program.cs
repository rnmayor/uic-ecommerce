using Ecommerce.Api.Extensions;
using Ecommerce.Api.Middleware;
using Ecommerce.Api.Security;
using Ecommerce.Application.Common.Authentication;
using Ecommerce.Application.Common.Authorization.Policies;
using Ecommerce.Application.Common.Interfaces;
using Ecommerce.Infrastructure;
using Ecommerce.Infrastructure.Identity;
using Ecommerce.Infrastructure.Tenancy;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;

// Bootstrap the entire application host and setup configuration, logging, DI, environment detection
var builder = WebApplication.CreateBuilder(args);

// Replace default logging
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithEnvironmentName()
        .Enrich.WithThreadId();
});

// Add services to the container
// Register all infrastructure services (DbContext, repositories, EF Core, etc.) via options pattern
builder.Services.AddInfrastructure(builder.Configuration);

// Register JWT Authentication
var clerkSection = builder.Configuration.GetSection("Authentication:Clerk");
builder.Services.Configure<ClerkAuthOptions>(clerkSection);

var clerkOptions = clerkSection.Get<ClerkAuthOptions>()!;
// Prevent claim type remapping
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = clerkOptions.Issuer;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = clerkOptions.Issuer,

            ValidateAudience = false, // Clerk does not require Audience
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            NameClaimType = "sub"
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            }
        };
    });


builder.Services.AddTenantServices();
builder.Services.AddUserServices();

// Register authorization policy "TenantAdmin" that allow users who are members of the current tenant with "Owner" or "Admin" role
// Enforced via TenantMemberAuthorizationHandler
builder.Services.AddAuthorization(options =>
{
    AuthorizationPolicies.AddPolicies(options);
});

// Enables MVC Controllers for REST API endpoints
builder.Services.AddControllers();
// Register endpoint metadata for Swagger/OpenAPI documentation generation
builder.Services.AddEndpointsApiExplorer();
// Register Swagger/OpenAPI generator to produce API documentation and UI
builder.Services.AddSwaggerGen();

// Finalize the DI container, locks configuration, and builds the middleware pipeline host
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Exposes Swagger only in Development
    app.UseSwagger();
    // Provides interactive UI to browse and test endpoints
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();

app.UseCorrelationId();
app.UseTenantResolution();
app.UseLogEnrichment();
app.UseGlobalExceptionHandling();

// Enforces access control and policies based on authenticated user's claims and roles
app.UseAuthorization();
// Map all API controller routes
app.MapControllers();

// Starts the web server and process incoming HTTP requests
app.Run();
