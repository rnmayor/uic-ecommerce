using Ecommerce.Domain.Common;

namespace Ecommerce.Domain.Stores;

public class StoreInstance : TenantEntity
{
    public Guid StoreBrandId { get; private set; }
    public StoreBrand StoreBrand { get; private set; } = default!;
    public string DisplayName { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    private StoreInstance() { } // For EF
    public StoreInstance(Guid tenantId, Guid storeBrandId, string displayName)
    {
        if (tenantId == Guid.Empty)
        {
            throw new DomainException("TenantId is required.");
        }
        if (storeBrandId == Guid.Empty)
        {
            throw new DomainException("StoreBrandId is required.");
        }
        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new DomainException("Store display name is required.");
        }

        Id = Guid.NewGuid();
        TenantId = tenantId;
        StoreBrandId = storeBrandId;
        DisplayName = displayName.Trim();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }
}
