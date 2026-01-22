using Ecommerce.Api.Middleware;
using Ecommerce.Application.Common.Authentication;
using Ecommerce.Application.Common.Tenancy;
using Ecommerce.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

// Bootstrap the entire application host and setup configuration, loggind, DI, environment detection
var builder = WebApplication.CreateBuilder(args);

// Add services to the container. Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

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


// Register all infrastracture services (DbContext, repositories, EF Core, etc.) via options pattern
builder.Services.AddInfrastracture(builder.Configuration);
// Register request-scoped tenant context to hold current tenant information
// Ensures each HTTP request has its own tenant instance, isolated from other requests
builder.Services.AddScoped<ITenantContext, TenantContext>();
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
// Custom middleware to resolve current tenant based on authenticated user
app.UseMiddleware<TenantResolutionMiddleware>();
// Enforces access control and policies based on authenticated user's claims and roles
app.UseAuthorization();
// Map all API controller routes
app.MapControllers();

// Starts the web server and process incoming HTTP requests
app.Run();
