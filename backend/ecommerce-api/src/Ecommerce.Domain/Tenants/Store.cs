using Ecommerce.Domain.Common;

namespace Ecommerce.Domain.Tenants;

public class Store : Entity
{
    public Guid TenantId { get; private set; }
    public string Name { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    private Store() { } // For EF
    public Store(Guid tenantId, string name)
    {
        if (tenantId == Guid.Empty)
        {
            throw new DomainException("Store must belong to a tenant.");
        }
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Store name is required.");
        }

        Id = Guid.NewGuid();
        TenantId = tenantId;
        Name = name.Trim();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }
}
