using Ecommerce.Application.Common.Persistence;
using Ecommerce.Domain.Common;
using Ecommerce.Domain.Tenants;

namespace Ecommerce.Application.Admin.Tenants.Onboarding;

public interface IOnboardingRepository
{
    Task<bool> UserAlreadyHasTenantAsync(Guid userId, CancellationToken ct = default);

}

public sealed class OnboardingService : IOnboardingService
{
    private readonly IOnboardingRepository _onboardingRepo;
    private readonly ITenantRepository _tenantRepo;
    public OnboardingService(IOnboardingRepository onboardingRepo, ITenantRepository tenantRepo)
    {
        _onboardingRepo = onboardingRepo;
        _tenantRepo = tenantRepo;
    }
    public async Task<OnboardingResponse> ExecuteAsync(Guid userId, OnboardingRequest request, CancellationToken ct = default)
    {
        if (await _onboardingRepo.UserAlreadyHasTenantAsync(userId, ct))
        {
            throw new DomainException("User already owns a tenant");
        }

        var tenant = new Tenant(request.TenantName, userId);
        var tenantUser = new TenantUser(tenant.Id, userId, TenantRoles.Owner);

        await _tenantRepo.CreateAsync(tenant, tenantUser, ct);

        return new OnboardingResponse
        {
            TenantId = tenant.Id
        };
    }
}
