using Ecommerce.Domain.Common;
using Ecommerce.Domain.Users;

namespace Ecommerce.Domain.Tenants
{
    public class TenantUser : TenantEntity
    {
        public Tenant Tenant { get; private set; } = default!;
        public Guid UserId { get; private set; }
        public User User { get; private set; } = default!;
        public string Role { get; private set; } = default!;
        public DateTime CreatedAt { get; private set; }

        private TenantUser() { } // For EF

        public static Result<TenantUser> Create(Guid tenantId, Guid userId, string role)
        {
            if (tenantId == Guid.Empty)
            {
                return TenantUserErrors.TenantRequired;
            }
            if (userId == Guid.Empty)
            {
                return TenantUserErrors.UserRequired;
            }
            if (string.IsNullOrWhiteSpace(role))
            {
                return TenantUserErrors.RoleRequired;
            }
            if (!TenantRoles.All.Contains(role))
            {
                return TenantUserErrors.TenantRoleInvalid;
            }

            var tenantUser = new TenantUser
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                UserId = userId,
                Role = role.Trim(),
                CreatedAt = DateTime.UtcNow,
            };

            return tenantUser;
        }
    }
}
