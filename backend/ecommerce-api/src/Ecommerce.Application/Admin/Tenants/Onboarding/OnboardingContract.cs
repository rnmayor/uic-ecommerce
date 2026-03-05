namespace Ecommerce.Application.Admin.Tenants.Onboarding;

public record OnboardingResponse
{
    public Guid TenantId { get; init; }
}

public record OnboardingRequest
{
    public string TenantName { get; init; } = default!;
}
