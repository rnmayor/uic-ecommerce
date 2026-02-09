using Ecommerce.Api.Extensions;
using Ecommerce.Application;
using Ecommerce.Application.Common.Authorization.Policies;
using Ecommerce.Infrastructure;
using Ecommerce.Infrastructure.Persistence;
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
builder.Services.AddAuthentication(builder.Configuration);

// Identity & Tenancy context
builder.Services.AddIdentity();
builder.Services.AddTenancy();

// Infrastructure (DB, repositories, authorization helpers)
builder.Services.AddInfrastructure();

// Authorization
builder.Services.AddAuthorization(options =>
{
    AuthorizationPolicies.AddPolicies(options);
});

// Mapping
builder.Services.AddAutoMapper();

// Application services (business logic)
builder.Services.AddApplicationServices();

// Health checks - for monitoring apps and DB connectivity
builder.Services.AddHealthChecks()
    .AddDbContextCheck<EcommerceDbContext>();

// API surface
builder.Services.AddApiControllers();
builder.Services.AddSwaggerDocumentation();

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

// Transport / protocol
app.UseHttpsRedirection();

app.UseCorrelationId();
app.UseGlobalExceptionHandling();

app.UseAuthentication();
app.UseTenantResolution();
app.UseAuthorization();
app.UseLogEnrichment();

// Endpoints / API controller routes
app.MapHealthChecks("/health");
app.MapControllers();

// Starts the web server and process incoming HTTP requests
app.Run();
