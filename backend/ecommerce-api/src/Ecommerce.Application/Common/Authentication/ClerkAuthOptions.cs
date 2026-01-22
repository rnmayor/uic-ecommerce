namespace Ecommerce.Application.Common.Authentication;

public sealed class ClerkAuthOptions
{
    public string Issuer { get; init; } = default!;
    public string? Audience { get; init; }
}
