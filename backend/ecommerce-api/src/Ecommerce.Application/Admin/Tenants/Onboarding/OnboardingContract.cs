using Ecommerce.Domain.Common;

namespace Ecommerce.Application.Admin.Tenants.Onboarding
{
    public sealed record OnboardingResponse
    {
        public Guid TenantId { get; init; }
    }

    public sealed record OnboardingRequest
    {
        public string TenantName { get; init; } = default!;
    }

    public interface IOnboardingRepository
    {
        Task<bool> UserAlreadyHasTenantAsync(Guid userId, CancellationToken ct = default);

    }

    public interface IOnboardingService
    {
        Task<Result<OnboardingResponse>> ExecuteAsync(Guid userId, OnboardingRequest request, CancellationToken ct = default);
    }
}