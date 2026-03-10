namespace Ecommerce.Application.Admin.Tenants.Onboarding;

public sealed record OnboardingResponse
{
    public Guid TenantId { get; init; }
}

public sealed record OnboardingRequest
{
    public string TenantName { get; init; } = default!;
}
