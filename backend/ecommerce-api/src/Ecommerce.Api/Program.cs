using Ecommerce.Api.Extensions;
using Ecommerce.Application.Common.Authorization.Policies;
using Ecommerce.Infrastructure;
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

// ---------- Add services to the container ---------- //

// Database configuration
builder.Services.AddDatabaseOptions(builder.Configuration);

// Global JWT behavior - execute once at startup. Disable all automatic claim remapping made by ASP.Net Core.
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

// Authentication
builder.Services.AddAuthenticationServices(builder.Configuration);

// Infrastructure and application services
builder.Services.AddInfrastructure();
builder.Services.AddTenantServices();
builder.Services.AddUserServices();

// Authorization
builder.Services.AddAuthorization(options =>
{
    AuthorizationPolicies.AddPolicies(options);
});

// MVC and API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
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

// Custom Middlewares
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
