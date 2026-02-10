using Ecommerce.Domain.Common;
using Ecommerce.Domain.Tenants;

namespace Ecommerce.Application.Admin.Tenants.Onboarding;

public sealed class CreateTenantService : ICreateTenantService
{
    public readonly ITenantOnboardingRepository _repository;
    public CreateTenantService(ITenantOnboardingRepository repository)
    {
        _repository = repository;
    }

    public async Task<CreateTenantResponse> CreateAsync(Guid userId, CreateTenantRequest request, CancellationToken ct = default)
    {
        if (await _repository.UserOwnsTenantAsync(userId, ct))
        {
            throw new DomainException("User already owns a tenant.");
        }

        var tenant = new Tenant(request.TenantName, ownerUserId: userId);
        var tenantUser = new TenantUser(tenant.Id, userId, TenantRoles.Owner);

        await _repository.CreateTenantAsync(tenant, tenantUser, ct);

        return new CreateTenantResponse
        {
            TenantId = tenant.Id,
        };
    }
}
