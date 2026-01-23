using Ecommerce.Api.Extensions;
using Ecommerce.Api.Logging;
using Ecommerce.Api.Middleware;
using Ecommerce.Api.Security;
using Ecommerce.Application.Common.Authentication;
using Ecommerce.Application.Common.Authorization;
using Ecommerce.Application.Common.Identity;
using Ecommerce.Application.Common.Tenancy;
using Ecommerce.Infrastructure;
using Ecommerce.Infrastructure.Identity;
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
// Register all infrastracture services (DbContext, repositories, EF Core, etc.) via options pattern
builder.Services.AddInfrastracture(builder.Configuration);

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

// Register request-scoped tenant context to hold current tenant information
// Ensures each HTTP request has its own tenant instance, isolated from other requests
builder.Services.AddScoped<ITenantContext, TenantContext>();
// Register the service to resolves application's internal User ID from Clerk's user ID
// Ensures mapping the authenticated Clerk user to a local User entity in the database
builder.Services.AddScoped<IUserResolver, UserResolver>();
// Register a claims transformation service for additional claims (user_id) to the authenticated user
// Allows downstream services, middleware, and authorization policies to rely on enriched claims
builder.Services.AddScoped<IClaimsTransformation, UserClaimsTransformation>();

// Register authorization policy "TenantAdmin" that allow users who are members of the current tenant with "Owner" or "Admin" role
// Enforced via TenantMemberAuthorizationHandler
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("TenantAdmin", policy => policy.Requirements.Add(new TenantMemberRequirement("Owner", "Admin")));
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

// Redirect all incoming HTTP requests to HTTPS to ensure secure communication
app.UseHttpsRedirection();
// Authenticate incoming requests using configured authentication schemes (JWT via Clerk)
app.UseAuthentication();
// Custom middleware to attach a unique identifer that travels with a request and shared across logs, errors, and downstream calls
app.UseMiddleware<CorrelationIdMiddleware>();
// Custom middleware to resolve current tenant based on authenticated user
app.UseMiddleware<TenantResolutionMiddleware>();
// Custom middleware for logging
app.UseMiddleware<LogEnrichmentMiddleware>();
// Custom middleware for global error handling
app.UseGlobalExceptionHandling();
// Enforces access control and policies based on authenticated user's claims and roles
app.UseAuthorization();
// Map all API controller routes
app.MapControllers();

// Starts the web server and process incoming HTTP requests
app.Run();
