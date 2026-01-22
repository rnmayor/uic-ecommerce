using Ecommerce.Domain.Common;

namespace Ecommerce.Domain.Tenants;

public class TenantUser : Entity
{
    public Guid TenantId { get; private set; }
    public Guid UserId { get; private set; }
    public string Role { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; }

    private TenantUser() { } // For EF

    public TenantUser(Guid tenantId, Guid userId, string role)
    {
        if (tenantId == Guid.Empty)
        {
            throw new DomainException("TenantId is required.");
        }
        if (userId == Guid.Empty)
        {
            throw new DomainException("UserId is required.");
        }
        if (string.IsNullOrWhiteSpace(role))
        {
            throw new DomainException("Role is required.");
        }

        Id = Guid.NewGuid();
        TenantId = tenantId;
        UserId = userId;
        Role = role;
        CreatedAt = DateTime.UtcNow;
    }
}
