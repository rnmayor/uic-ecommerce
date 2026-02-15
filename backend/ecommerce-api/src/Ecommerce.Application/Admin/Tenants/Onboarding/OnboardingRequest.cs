namespace Ecommerce.Application.Admin.Tenants.Onboarding;

public sealed class OnboardingRequest
{
    public string TenantName { get; init; } = default!;
}
