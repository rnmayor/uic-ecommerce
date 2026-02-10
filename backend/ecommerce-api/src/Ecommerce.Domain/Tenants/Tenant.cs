using Ecommerce.Domain.Common;

namespace Ecommerce.Domain.Tenants;

public class Tenant : Entity
{
    public string Name { get; private set; } = default!;
    public Guid OwnerUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Tenant() { } // For EF

    public Tenant(string name, Guid ownerUserId)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Tenant name is required.");
        }
        if (ownerUserId == Guid.Empty)
        {
            throw new DomainException("OwnerUserId is required");
        }

        Id = Guid.NewGuid();
        Name = name.Trim();
        OwnerUserId = ownerUserId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }
}
