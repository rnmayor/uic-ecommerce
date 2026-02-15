using Ecommerce.Domain.Tenants;

namespace Ecommerce.Application.Common.Persistence;

public interface ITenantRepository
{
    Task CreateAsync(Tenant tenant, TenantUser tenantUser, CancellationToken ct = default);
}
