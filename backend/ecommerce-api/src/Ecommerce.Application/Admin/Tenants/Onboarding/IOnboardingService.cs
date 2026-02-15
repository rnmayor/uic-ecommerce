namespace Ecommerce.Application.Admin.Tenants.Onboarding;

public interface IOnboardingService
{
    Task<OnboardingResponse> ExecuteAsync(Guid userId, OnboardingRequest request, CancellationToken ct = default);
}
