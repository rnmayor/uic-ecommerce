namespace Ecommerce.Application.Common.Tenancy;

public interface ITenantContext
{
    Guid TenantId { get; }
    bool IsResolved { get; }
    void SetTenant(Guid tenantId);
}
