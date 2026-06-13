using Ecommerce.Application.Common.Interfaces;

namespace Ecommerce.Infrastructure.Tenancy;

public sealed class TenantContext : ITenantContext
{
    public Guid TenantId { get; private set; }

    public bool IsResolved => TenantId != Guid.Empty;

    public void SetTenant(Guid tenantId)
    {
        TenantId = tenantId;
    }
}
