using Ecommerce.Domain.Common;
using Ecommerce.Domain.Users;

namespace Ecommerce.Domain.Tenants;

public class TenantUser : TenantEntity
{
    public Tenant Tenant { get; private set; } = default!;
    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;
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
        if (!TenantRoles.All.Contains(role))
        {
            throw new DomainException($"Invalid tenant role: {role}");
        }

        Id = Guid.NewGuid();
        TenantId = tenantId;
        UserId = userId;
        Role = role.Trim();
        CreatedAt = DateTime.UtcNow;
    }
}
