using Ecommerce.Infrastructure;

// Bootstrap the entire application host and setup configuration, loggind, DI, environment detection
var builder = WebApplication.CreateBuilder(args);

// Add services to the container. Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

// Call extension method registered in IServiceCollection defined in Ecommerce.Infrastructure.DependencyInjection
builder.Services.AddInfrastracture(builder.Configuration);
// Enables MVC Controllers for REST APIs
builder.Services.AddControllers();
// Scans controllers and endpoints and produces metadata for Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
// Register Swagger generator, builds OpenAPI JSON
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

// Redirect HTTP to HTTPS for security
app.UseHttpsRedirection();
// Enable authorization middleware, enforces [Authorize], Policies, Roles/claims
app.UseAuthorization();
// Register all controller routes
app.MapControllers();

// Starts the web server and end of the pipeline
app.Run();
