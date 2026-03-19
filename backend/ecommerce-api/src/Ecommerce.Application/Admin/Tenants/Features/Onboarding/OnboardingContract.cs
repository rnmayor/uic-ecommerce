using Ecommerce.Domain.Common;
using Ecommerce.Domain.Tenants;

namespace Ecommerce.Application.Admin.Tenants.Features.Onboarding
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
        Task<bool> TenantExistAsync(string normalizedName, CancellationToken ct = default);
        Task CreateTenantWithOwnerAsync(Tenant tenant, TenantUser owner, CancellationToken ct = default);
    }

    public interface IOnboardingService
    {
        Task<Result<OnboardingResponse>> ExecuteAsync(Guid userId, OnboardingRequest request, CancellationToken ct = default);
    }
}