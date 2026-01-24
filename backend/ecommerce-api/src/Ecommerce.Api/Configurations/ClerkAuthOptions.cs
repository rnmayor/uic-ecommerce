using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Api.Configurations;

public sealed class ClerkAuthOptions
{
    public const string SectionName = "Authentication:Clerk";

    [Required]
    public string Issuer { get; init; } = default!;
    public string? Audience { get; init; }
}
