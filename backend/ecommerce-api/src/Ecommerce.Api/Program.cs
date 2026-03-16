using Ecommerce.Api.Errors;
using Ecommerce.Api.Extensions;
using Ecommerce.Application;
using Ecommerce.Application.Common.Authorization.Policies;
using Ecommerce.Infrastructure;
using Ecommerce.Infrastructure.Persistence;
using Serilog;
using System.IdentityModel.Tokens.Jwt;

// Bootstrap the entire application host and setup configuration, logging, DI, environment detection
var builder = WebApplication.CreateBuilder(args);

// Standardize port binding for Cloud proivders (Railway/Azure)
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://*:{port}");

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

// Infrastructure (DB, repositories, authorization helpers)
builder.Services.AddInfrastructure();

// Identity & Tenancy context
builder.Services.AddIdentity();
builder.Services.AddTenancy();

// Authorization
builder.Services.AddAuthorization(options =>
{
    AuthorizationPolicies.AddPolicies(options);
});

// Application services (business logic)
builder.Services.AddApplicationServices();

// Health checks - for monitoring apps and DB connectivity
builder.Services.AddHealthChecks()
    .AddDbContextCheck<EcommerceDbContext>();

// API controllers and Swagger documentation
builder.Services.AddApiControllers();
builder.Services.AddSwaggerDocumentation();

// Global error handling - catches unhandled exceptions and converts them to standardized API error responses
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Finalize the DI container, locks configuration, and builds the middleware pipeline host
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.UseCorrelationId();

app.UseAuthentication();
app.UseTenantResolution();
app.UseAuthorization();

app.UseLogEnrichment();

app.MapHealthChecks("/health");
app.MapControllers();

app.Run();
