using Ecommerce.Domain.Tenants;

namespace Ecommerce.Application.Admin.Tenants.Onboarding;

public interface ITenantOnboardingRepository
{
    Task<bool> UserOwnsTenantAsync(Guid userId, CancellationToken ct);
    Task CreateTenantAsync(
      Tenant tenant,
      TenantUser owner,
      CancellationToken ct = default
    );
}
