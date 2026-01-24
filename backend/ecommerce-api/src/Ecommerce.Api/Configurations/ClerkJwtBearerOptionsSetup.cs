using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Ecommerce.Api.Configurations;

internal sealed class ClerkJwtBearerOptionsSetup : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly ClerkAuthOptions _options;
    public ClerkJwtBearerOptionsSetup(IOptions<ClerkAuthOptions> options)
    {
        _options = options.Value;
    }
    public void Configure(string? name, JwtBearerOptions options)
    {
        if (name != JwtBearerDefaults.AuthenticationScheme)
        {
            return;
        }

        options.Authority = _options.Issuer;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _options.Issuer,
            ValidateAudience = false, // Clerk does not require Audience
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            NameClaimType = "sub"
        };
    }

    public void Configure(JwtBearerOptions options)
    {
        Configure(JwtBearerDefaults.AuthenticationScheme, options);
    }
}
